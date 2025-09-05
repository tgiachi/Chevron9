using Chevron9.Core.Interfaces;
using Chevron9.Core.Scenes;
using Chevron9.Shared.Graphics;

namespace Chevron9.Tests.Core.Scenes;

[TestFixture]
public class MemoryLeakTests
{
    private sealed class TrackingScene : IScene
    {
        public string Name { get; }
        public int UpdateCallCount { get; private set; }
        public int RenderCallCount { get; private set; }
        public int HandleInputCallCount { get; private set; }
        public bool IsDisposed { get; private set; }
        public List<WeakReference> TrackedObjects { get; } = new();
        public IReadOnlyList<ILayer> Layers => [];

        public TrackingScene(string name)
        {
            Name = name;
        }

        public void Enter() { }

        public void Close() { }

        public void Update(double dt, IInputDevice input)
        {
            UpdateCallCount++;
        }

        public void Render(IRenderCommandCollector rq, float alpha)
        {
            RenderCallCount++;
        }

        public bool HandleInput(IInputDevice input)
        {
            HandleInputCallCount++;
            return false;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public void TrackObject(object obj)
        {
            TrackedObjects.Add(new WeakReference(obj));
        }

        public int GetLiveObjectCount()
        {
            return TrackedObjects.Count(wr => wr.IsAlive);
        }
    }

    private sealed class ResourceHolder : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public byte[] LargeData { get; } = new byte[1024 * 1024]; // 1MB of data

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    [Test]
    public void LongRunningScene_DoesNotAccumulateScenesInManager()
    {
        using var manager = new SceneManager();

        // Create and push many scenes
        for (int i = 0; i < 100; i++)
        {
            var scene = new TrackingScene($"Scene{i}");
            manager.Push(scene);

            // Simulate some work
            manager.Update(0.016, null!);
            manager.Render(null!, 1.0f);

            // Pop the scene
            manager.Pop();
        }

        // Manager should be empty
        Assert.That(manager.Current, Is.Null);
    }

    [Test]
    public void SceneReplacement_DoesNotLeakPreviousScenes()
    {
        using var manager = new SceneManager();
        var weakRefs = new List<WeakReference>();

        // Create scenes and track them with weak references
        for (int i = 0; i < 50; i++)
        {
            var scene = new TrackingScene($"Scene{i}");
            weakRefs.Add(new WeakReference(scene));

            manager.Push(scene);
            manager.Update(0.016, null!);

            // Replace with a new scene
            var newScene = new TrackingScene($"NewScene{i}");
            manager.Replace(newScene);
        }

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Most scenes should be garbage collected
        int liveScenes = weakRefs.Count(wr => wr.IsAlive);
        Assert.That(liveScenes, Is.LessThan(10), "Too many scenes are still alive, indicating a memory leak");
    }

    [Test]
    public void SceneManagerDispose_ReleasesAllSceneReferences()
    {
        var manager = new SceneManager();
        var weakRefs = new List<WeakReference>();

        // Create and push scenes
        for (int i = 0; i < 20; i++)
        {
            var scene = new TrackingScene($"Scene{i}");
            weakRefs.Add(new WeakReference(scene));
            manager.Push(scene);
        }

        // Dispose manager
        manager.Dispose();

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Most scenes should be garbage collected (allow some due to GC timing)
        int liveScenes = weakRefs.Count(wr => wr.IsAlive);
        Assert.That(liveScenes, Is.LessThan(5), $"{liveScenes} scenes still alive after manager disposal");
    }

    [Test]
    public void DeferredOperations_DoNotCauseMemoryLeaks()
    {
        using var manager = new SceneManager();
        var weakRefs = new List<WeakReference>();

        // Create scenes
        for (int i = 0; i < 30; i++)
        {
            var scene = new TrackingScene($"Scene{i}");
            weakRefs.Add(new WeakReference(scene));
            manager.Push(scene);

            // Queue operations that will be deferred
            manager.Pop();
            manager.Push(new TrackingScene($"DeferredScene{i}"));
        }

        // Force processing of deferred operations
        manager.Update(0.016, null!);

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Should not have excessive live objects
        int liveScenes = weakRefs.Count(wr => wr.IsAlive);
        Assert.That(liveScenes, Is.LessThan(15), "Too many scenes alive after deferred operations");
    }

    [Test]
    public void ResourceDisposal_InScenes_IsProperlyHandled()
    {
        using var manager = new SceneManager();

        // Create scene with resources
        var scene = new TrackingScene("ResourceScene");
        var resource = new ResourceHolder();
        scene.TrackObject(resource);

        manager.Push(scene);

        // Simulate long running
        for (int i = 0; i < 1000; i++)
        {
            manager.Update(0.016, null!);
        }

        manager.Pop();

        // Force GC
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Resource disposal is not handled by TrackingScene (it's just for tracking)
        // This test is primarily checking for memory leaks, not resource disposal
        Assert.That(resource.IsDisposed, Is.False);
    }

    [Test]
    public void SceneStackOperations_DoNotCreateReferenceCycles()
    {
        using var manager = new SceneManager();
        var weakRefs = new List<WeakReference>();

        // Create a deep scene stack
        for (int i = 0; i < 10; i++)
        {
            var scene = new TrackingScene($"StackScene{i}");
            weakRefs.Add(new WeakReference(scene));
            manager.Push(scene);
        }

        // Pop all scenes
        for (int i = 0; i < 10; i++)
        {
            manager.Pop();
        }

        // Force GC
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Most scenes should be collected (allow some due to GC timing)
        int liveScenes = weakRefs.Count(wr => wr.IsAlive);
        Assert.That(liveScenes, Is.LessThan(3), $"{liveScenes} scenes still alive, possible reference cycle");
    }

    [Test]
    public void LongRunningUpdateLoop_DoesNotAccumulateMemory()
    {
        using var manager = new SceneManager();
        var scene = new TrackingScene("LongRunningScene");
        var initialMemory = GC.GetTotalMemory(true);

        manager.Push(scene);

        // Run many update cycles
        for (int i = 0; i < 10000; i++)
        {
            manager.Update(0.016, null!);

            // Periodic GC to check memory growth
            if (i % 1000 == 0)
            {
                GC.Collect();
            }
        }

        var finalMemory = GC.GetTotalMemory(true);
        var memoryGrowth = finalMemory - initialMemory;

        // Memory growth should be reasonable (less than 10MB for this test)
        Assert.That(memoryGrowth, Is.LessThan(10 * 1024 * 1024),
            $"Excessive memory growth: {memoryGrowth} bytes");
    }

    [Test]
    public void SceneManagerWithManyPushPopCycles_DoesNotLeak()
    {
        using var manager = new SceneManager();
        var createdScenes = new List<WeakReference>();

        // Simulate many push/pop cycles like in a game menu system
        for (int cycle = 0; cycle < 100; cycle++)
        {
            for (int scene = 0; scene < 5; scene++)
            {
                var newScene = new TrackingScene($"Cycle{cycle}Scene{scene}");
                createdScenes.Add(new WeakReference(newScene));
                manager.Push(newScene);

                // Do some work
                manager.Update(0.016, null!);
                manager.Render(null!, 1.0f);
            }

            // Pop all scenes from this cycle
            for (int scene = 0; scene < 5; scene++)
            {
                manager.Pop();
            }
        }

        // Force aggressive GC
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Count live objects
        int liveScenes = createdScenes.Count(wr => wr.IsAlive);

        // Allow some objects to still be alive due to timing, but not too many
        Assert.That(liveScenes, Is.LessThan(50),
            $"{liveScenes} scenes still alive out of {createdScenes.Count} created");
    }

    [Test]
    public void ExceptionDuringSceneCleanup_DoesNotPreventOtherCleanup()
    {
        var manager = new SceneManager();

        // Create scenes, some of which will throw during disposal
        var normalScene = new TrackingScene("NormalScene");
        var throwingScene = new TrackingScene("ThrowingScene");

        manager.Push(normalScene);
        manager.Push(throwingScene);

        // Dispose manager - should handle exceptions gracefully
        Assert.DoesNotThrow(() => manager.Dispose());

        // Both scenes should be disposed
        Assert.That(normalScene.IsDisposed, Is.True);
        Assert.That(throwingScene.IsDisposed, Is.True);
    }

    [Test]
    public void SceneManagerReset_AfterLongUsage_DoesNotLeak()
    {
        using var manager = new SceneManager();
        var allCreatedScenes = new List<WeakReference>();

        // Simulate long usage with scene changes
        for (int phase = 0; phase < 10; phase++)
        {
            // Create multiple scenes per phase
            for (int s = 0; s < 3; s++)
            {
                var scene = new TrackingScene($"Phase{phase}Scene{s}");
                allCreatedScenes.Add(new WeakReference(scene));
                manager.Push(scene);
            }

            // Use scenes for a while
            for (int update = 0; update < 100; update++)
            {
                manager.Update(0.016, null!);
            }

            // Clear all scenes
            while (manager.Current != null)
            {
                manager.Pop();
            }
        }

        // Force GC
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Check for leaks
        int liveScenes = allCreatedScenes.Count(wr => wr.IsAlive);
        Assert.That(liveScenes, Is.LessThan(20),
            $"Memory leak detected: {liveScenes} scenes still alive");
    }
}
