using ReCVEServer.Networking.Events;

namespace ReCVEServer.Networking.ClientProcessing {
    public class AssemblySender {
        public delegate void AssemblyEventHandler(object sender, AssemblyEvent args);
        public event AssemblyEventHandler RaiseAssemblyEvent;

        public async Task SendAssembly(int ID, string assemblyName, string assemblyData, List<string> args) {                        
            OnRaiseAssemblyEvent(new AssemblyEvent(ID, assemblyName, assemblyData, args));         
            await Task.CompletedTask;
        }

        protected virtual void OnRaiseAssemblyEvent(AssemblyEvent eventArgs) {
            AssemblyEventHandler raiseEvent = RaiseAssemblyEvent;           
            if (raiseEvent != null) {               
                raiseEvent(this, eventArgs);
            }
        }
    }

}
