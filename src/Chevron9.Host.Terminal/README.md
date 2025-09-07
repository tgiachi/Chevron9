# Chevron9.Host.Terminal

Host completo per applicazioni terminal che combina Chevron9.Bootstrap con i servizi del backend terminal (Chevron9.Backends.Terminal).

## Caratteristiche

- 🔧 **Integrazione completa**: Combina Bootstrap e backend terminal in un unico host
- ⚡ **Gestione servizi**: Caricamento e avvio automatico dei servizi con priorità
- 🎨 **Rendering**: Supporto per rendering in tempo reale con double buffering
- 🖱️ **Input**: Gestione di input da tastiera e mouse con eventi asincroni
- 🔄 **Lifecycle**: Gestione completa del ciclo di vita dell'applicazione
- ⚙️ **Configurabile**: Configurazione flessibile per diverse esigenze
- 📝 **Logging**: Integrazione con Serilog per logging strutturato

## Utilizzo Rapido

### Applicazione Base

```csharp
using Chevron9.Host.Terminal.Extensions;

// Crea e avvia un'applicazione terminal
using var host = Chevron9TerminalHostExtensions.CreateDefaultHost();

// Registra i tuoi servizi
host.OnRegisterServices += container =>
{
    container.AddService<MyAppService>(priority: 50);
};

// Gestione graceful di Ctrl+C
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

// Avvia l'applicazione
await host.RunAsync(cts.Token);
```

### Configurazione Personalizzata

```csharp
using var host = Chevron9TerminalHostExtensions.CreateHost(config =>
{
    config.RootDirectory = "./my-app";
    config.LogLevel = LogLevelType.Information;
    config.RefreshRateMs = 33; // 30 FPS
    config.EnableMouse = true;
    config.ShowCursor = false;
});
```

### Configurazione Fluent

```csharp
using var host = Chevron9TerminalHostExtensions.CreateDefaultHost()
    .WithRefreshRate(16)     // 60 FPS
    .WithMouse(true)         // Abilita mouse
    .WithCursor(false)       // Nascondi cursore
    .WithDimensions(80, 24); // Dimensioni fisse
```

## Builder Preconfigurati

### Development Host
```csharp
var host = Chevron9TerminalHostExtensions.CreateDevelopmentHost("./dev-app");
// - LogLevel: Debug
// - LogToFile: true
// - ShowCursor: true
// - RefreshRate: 30 FPS
```

### Production Host
```csharp
var host = Chevron9TerminalHostExtensions.CreateProductionHost("./prod-app");
// - LogLevel: Warning
// - LogToFile: true
// - ShowCursor: false
// - RefreshRate: 60 FPS
```

## Controllo Manuale

```csharp
using var host = Chevron9TerminalHostExtensions.CreateDefaultHost();

host.OnRegisterServices += container =>
{
    container.AddService<MyService>(priority: 50);
};

// Controllo manuale del lifecycle
await host.InitializeAsync();
await host.StartAsync();

// La tua logica applicativa qui
await DoApplicationWork();

// Shutdown
await host.StopAsync();
```

## Accesso ai Servizi Terminal

```csharp
using var host = Chevron9TerminalHostExtensions.CreateDefaultHost();
await host.InitializeAsync();

// Accesso ai servizi terminal (disponibili dopo InitializeAsync)
var renderService = host.RenderService;
var inputService = host.InputService;

// Accesso al renderer per draw commands
var renderer = renderService?.Renderer;
if (renderer != null)
{
    renderer.BeginFrame();
    // ... draw commands
    renderer.EndFrame();
}
```

## Creazione di Servizi Personalizzati

```csharp
using Chevron9.Bootstrap.Interfaces.Base;

public class MyAppService : IChevronAutostartService
{
    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        // Inizializzazione del servizio
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        // Avvio del servizio (autostart)
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        // Arresto del servizio
        return Task.CompletedTask;
    }

    public Task UnloadAsync(CancellationToken cancellationToken = default)
    {
        // Cleanup del servizio
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Dispose resources
    }
}
```

## Configurazione

### Chevron9TerminalConfig

- **EnableInput/EnableOutput**: Abilita/disabilita input e output
- **BufferWidth/BufferHeight**: Dimensioni buffer (0 = auto-detect)
- **RefreshRateMs**: Frequenza di refresh in millisecondi
- **EnableMouse**: Supporto mouse
- **ShowCursor**: Visibilità cursore
- **UseAlternativeScreenBuffer**: Buffer schermo alternativo
- Eredita tutte le opzioni di `Chevron9Config` (logging, directories, etc.)

### Mode Specializzati

```csharp
// Solo output (nessun input)
host.WithOutputOnly();

// Solo input (nessun rendering)  
host.WithInputOnly();

// Modalità headless (no I/O)
host.WithHeadlessMode();
```

## Architettura

```
Chevron9TerminalHost
├── Chevron9Bootstrap (service lifecycle management)
├── TerminalRenderService (ConsoleRender wrapper)
├── TerminalInputService (ConsoleInputDevice wrapper)
└── Your Application Services
```

### Priorità dei Servizi

- TerminalRenderService: priorità 100
- TerminalInputService: priorità 90  
- I tuoi servizi: priorità personalizzata
- EventDispatcherService: priorità 0 (built-in)

## Thread Safety

- ✅ Services lifecycle è thread-safe
- ✅ Event dispatching è thread-safe  
- ⚠️ ConsoleRender richiede sincronizzazione per chiamate concorrenti
- ⚠️ ConsoleInputDevice usa polling interno thread-safe

## Performance

- **Double Buffering**: Rendering efficiente senza flickering
- **1D Arrays**: Buffer ottimizzati per performance memoria
- **Configurable Refresh Rate**: Bilancia performance/responsiveness
- **Frame-based Input**: Polling ottimizzato per applicazioni real-time

## Logging

Il sistema di logging è configurato automaticamente:
- File di log salvati in `{RootDirectory}/Logs/`  
- Console logging disabilitato di default per evitare interferenze con l'UI terminal
- Livelli: Trace, Debug, Information, Warning, Error, Critical

## Esempi Completi

Vedi la cartella `Examples/` per implementazioni complete di:
- Applicazioni console interattive
- Game loops con rendering real-time
- Gestori di eventi personalizzati
- Integrazione con external services