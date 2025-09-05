using Chevron9.Core.Interfaces;
using Chevron9.Core.Scenes;

namespace Chevron9.Tests.Core.Scenes;

[TestFixture]
public class SceneManagerTests
{
    private sealed class TestScene : IScene
    {
        public string Name { get; }
        public bool EnterCalled { get; private set; }
        public bool CloseCalled { get; private set; }
        public bool UpdateCalled { get; private set; }
        public bool RenderCalled { get; private set; }
        public bool HandleInputCalled { get; private set; }
        public bool DisposeCalled { get; private set; }
        public IReadOnlyList<ILayer> Layers => [];

        public TestScene(string name)
        {
            Name = name;
        }

        public void Enter()
        {
            EnterCalled = true;
        }

        public void Close()
        {
            CloseCalled = true;
        }

        public void Update(double dt, IInputDevice input)
        {
            UpdateCalled = true;
        }

        public void Render(IRenderCommandCollector rq, float alpha)
        {
            RenderCalled = true;
        }

        public bool HandleInput(IInputDevice input)
        {
            HandleInputCalled = true;
            return false;
        }

        public void Dispose()
        {
            DisposeCalled = true;
        }
    }

    [SetUp]
    public void Setup()
    {
        // Reset any static state if needed
    }

    [Test]
    public void Constructor_CreatesEmptySceneManager()
    {
        using var manager = new SceneManager();

        Assert.That(manager.Current, Is.Null);
    }

    [Test]
    public void Push_WithValidScene_AddsToStack()
    {
        using var manager = new SceneManager();
        var scene = new TestScene("TestScene");

        manager.Push(scene);

        Assert.That(manager.Current, Is.EqualTo(scene));
        Assert.That(scene.EnterCalled, Is.True);
    }

    [Test]
    public void Push_WithNullScene_ThrowsArgumentNullException()
    {
        using var manager = new SceneManager();

        Assert.Throws<ArgumentNullException>(() => manager.Push(null!));
    }

    [Test]
    public void Pop_WithSceneOnStack_RemovesScene()
    {
        using var manager = new SceneManager();
        var scene = new TestScene("TestScene");
        manager.Push(scene);

        manager.Pop();

        Assert.That(manager.Current, Is.Null);
        Assert.That(scene.CloseCalled, Is.True);
        Assert.That(scene.DisposeCalled, Is.True);
    }

    [Test]
    public void Pop_WithEmptyStack_DoesNotThrow()
    {
        using var manager = new SceneManager();

        Assert.DoesNotThrow(() => manager.Pop());
    }

    [Test]
    public void Replace_WithSceneOnStack_ReplacesCurrentScene()
    {
        using var manager = new SceneManager();
        var oldScene = new TestScene("OldScene");
        var newScene = new TestScene("NewScene");
        manager.Push(oldScene);

        manager.Replace(newScene);

        Assert.That(manager.Current, Is.EqualTo(newScene));
        Assert.That(oldScene.CloseCalled, Is.True);
        Assert.That(oldScene.DisposeCalled, Is.True);
        Assert.That(newScene.EnterCalled, Is.True);
    }

    [Test]
    public void Replace_WithEmptyStack_AddsNewScene()
    {
        using var manager = new SceneManager();
        var scene = new TestScene("TestScene");

        manager.Replace(scene);

        Assert.That(manager.Current, Is.EqualTo(scene));
        Assert.That(scene.EnterCalled, Is.True);
    }

    [Test]
    public void Replace_WithNullScene_ThrowsArgumentNullException()
    {
        using var manager = new SceneManager();

        Assert.Throws<ArgumentNullException>(() => manager.Replace(null!));
    }

    [Test]
    public void Push_MultipleScenes_UpdatesCurrentToTop()
    {
        using var manager = new SceneManager();
        var scene1 = new TestScene("Scene1");
        var scene2 = new TestScene("Scene2");
        var scene3 = new TestScene("Scene3");

        manager.Push(scene1);
        manager.Push(scene2);
        manager.Push(scene3);

        Assert.That(manager.Current, Is.EqualTo(scene3));
        Assert.That(scene1.EnterCalled, Is.True);
        Assert.That(scene2.EnterCalled, Is.True);
        Assert.That(scene3.EnterCalled, Is.True);
    }

    [Test]
    public void Pop_MultipleScenes_RevealsPreviousScene()
    {
        using var manager = new SceneManager();
        var scene1 = new TestScene("Scene1");
        var scene2 = new TestScene("Scene2");
        var scene3 = new TestScene("Scene3");

        manager.Push(scene1);
        manager.Push(scene2);
        manager.Push(scene3);

        manager.Pop();

        Assert.That(manager.Current, Is.EqualTo(scene2));
        Assert.That(scene3.CloseCalled, Is.True);
        Assert.That(scene3.DisposeCalled, Is.True);
        Assert.That(scene2.CloseCalled, Is.False);
    }

    [Test]
    public void Update_WithCurrentScene_CallsUpdateOnCurrentScene()
    {
        using var manager = new SceneManager();
        var scene = new TestScene("TestScene");
        manager.Push(scene);

        manager.Update(0.016, null!);

        Assert.That(scene.UpdateCalled, Is.True);
    }

    [Test]
    public void Update_WithNoScenes_DoesNotThrow()
    {
        using var manager = new SceneManager();

        Assert.DoesNotThrow(() => manager.Update(0.016, null!));
    }

    [Test]
    public void Render_WithScenes_RendersAllScenesInOrder()
    {
        using var manager = new SceneManager();
        var scene1 = new TestScene("Scene1");
        var scene2 = new TestScene("Scene2");
        var scene3 = new TestScene("Scene3");

        manager.Push(scene1);
        manager.Push(scene2);
        manager.Push(scene3);

        manager.Render(null!, 1.0f);

        // All scenes should be rendered (bottom to top)
        Assert.That(scene1.RenderCalled, Is.True);
        Assert.That(scene2.RenderCalled, Is.True);
        Assert.That(scene3.RenderCalled, Is.True);
    }

    [Test]
    public void Render_WithNoScenes_DoesNotThrow()
    {
        using var manager = new SceneManager();

        Assert.DoesNotThrow(() => manager.Render(null!, 1.0f));
    }

    [Test]
    public void HandleInput_WithCurrentScene_CallsHandleInputOnCurrentScene()
    {
        using var manager = new SceneManager();
        var scene = new TestScene("TestScene");
        manager.Push(scene);

        var result = manager.HandleInput(null!);

        Assert.That(scene.HandleInputCalled, Is.True);
        Assert.That(result, Is.False); // Our test scene returns false
    }

    [Test]
    public void HandleInput_WithNoScenes_ReturnsFalse()
    {
        using var manager = new SceneManager();

        var result = manager.HandleInput(null!);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Dispose_WithScenes_DisposesAllScenes()
    {
        var manager = new SceneManager();
        var scene1 = new TestScene("Scene1");
        var scene2 = new TestScene("Scene2");

        manager.Push(scene1);
        manager.Push(scene2);

        manager.Dispose();

        Assert.That(scene1.CloseCalled, Is.True);
        Assert.That(scene1.DisposeCalled, Is.True);
        Assert.That(scene2.CloseCalled, Is.True);
        Assert.That(scene2.DisposeCalled, Is.True);
    }

    [Test]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        var manager = new SceneManager();

        manager.Dispose();

        Assert.DoesNotThrow(() => manager.Dispose());
    }

    [Test]
    public void OperationsDuringUpdate_AreDeferred()
    {
        using var manager = new SceneManager();
        var scene1 = new TestScene("Scene1");
        var scene2 = new TestScene("Scene2");
        manager.Push(scene1);

        // During update, operations should be deferred
        manager.Update(0.016, null!);

        // Push should be deferred and executed after update
        manager.Push(scene2);

        // Scene2 should now be current
        Assert.That(manager.Current, Is.EqualTo(scene2));
        Assert.That(scene2.EnterCalled, Is.True);
    }

    [Test]
    public void OperationsDuringRender_AreDeferred()
    {
        using var manager = new SceneManager();
        var scene1 = new TestScene("Scene1");
        var scene2 = new TestScene("Scene2");
        manager.Push(scene1);

        // During render, operations should be deferred
        manager.Render(null!, 1.0f);

        // Replace should be deferred and executed after render
        manager.Replace(scene2);

        // Scene2 should now be current
        Assert.That(manager.Current, Is.EqualTo(scene2));
        Assert.That(scene1.CloseCalled, Is.True);
        Assert.That(scene1.DisposeCalled, Is.True);
        Assert.That(scene2.EnterCalled, Is.True);
    }
}
