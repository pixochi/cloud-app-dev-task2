﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GUI.AWSSortMidServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AWSSortMidServiceReference.ISortMidService")]
    public interface ISortMidService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISortMidService/GetAllocations", ReplyAction="http://tempuri.org/ISortMidService/GetAllocationsResponse")]
        System.Collections.Generic.List<TaskAllocationLibrary.TaskAllocationOutput> GetAllocations(TaskAllocationLibrary.TaskAllocationInput input);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/ISortMidService/GetAllocations", ReplyAction="http://tempuri.org/ISortMidService/GetAllocationsResponse")]
        System.IAsyncResult BeginGetAllocations(TaskAllocationLibrary.TaskAllocationInput input, System.AsyncCallback callback, object asyncState);
        
        System.Collections.Generic.List<TaskAllocationLibrary.TaskAllocationOutput> EndGetAllocations(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISortMidServiceChannel : GUI.AWSSortMidServiceReference.ISortMidService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetAllocationsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetAllocationsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.Generic.List<TaskAllocationLibrary.TaskAllocationOutput> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.Generic.List<TaskAllocationLibrary.TaskAllocationOutput>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SortMidServiceClient : System.ServiceModel.ClientBase<GUI.AWSSortMidServiceReference.ISortMidService>, GUI.AWSSortMidServiceReference.ISortMidService {
        
        private BeginOperationDelegate onBeginGetAllocationsDelegate;
        
        private EndOperationDelegate onEndGetAllocationsDelegate;
        
        private System.Threading.SendOrPostCallback onGetAllocationsCompletedDelegate;
        
        public SortMidServiceClient() {
        }
        
        public SortMidServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SortMidServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SortMidServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SortMidServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<GetAllocationsCompletedEventArgs> GetAllocationsCompleted;
        
        public System.Collections.Generic.List<TaskAllocationLibrary.TaskAllocationOutput> GetAllocations(TaskAllocationLibrary.TaskAllocationInput input) {
            return base.Channel.GetAllocations(input);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetAllocations(TaskAllocationLibrary.TaskAllocationInput input, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetAllocations(input, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.Collections.Generic.List<TaskAllocationLibrary.TaskAllocationOutput> EndGetAllocations(System.IAsyncResult result) {
            return base.Channel.EndGetAllocations(result);
        }
        
        private System.IAsyncResult OnBeginGetAllocations(object[] inValues, System.AsyncCallback callback, object asyncState) {
            TaskAllocationLibrary.TaskAllocationInput input = ((TaskAllocationLibrary.TaskAllocationInput)(inValues[0]));
            return this.BeginGetAllocations(input, callback, asyncState);
        }
        
        private object[] OnEndGetAllocations(System.IAsyncResult result) {
            System.Collections.Generic.List<TaskAllocationLibrary.TaskAllocationOutput> retVal = this.EndGetAllocations(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetAllocationsCompleted(object state) {
            if ((this.GetAllocationsCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetAllocationsCompleted(this, new GetAllocationsCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetAllocationsAsync(TaskAllocationLibrary.TaskAllocationInput input) {
            this.GetAllocationsAsync(input, null);
        }
        
        public void GetAllocationsAsync(TaskAllocationLibrary.TaskAllocationInput input, object userState) {
            if ((this.onBeginGetAllocationsDelegate == null)) {
                this.onBeginGetAllocationsDelegate = new BeginOperationDelegate(this.OnBeginGetAllocations);
            }
            if ((this.onEndGetAllocationsDelegate == null)) {
                this.onEndGetAllocationsDelegate = new EndOperationDelegate(this.OnEndGetAllocations);
            }
            if ((this.onGetAllocationsCompletedDelegate == null)) {
                this.onGetAllocationsCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetAllocationsCompleted);
            }
            base.InvokeAsync(this.onBeginGetAllocationsDelegate, new object[] {
                        input}, this.onEndGetAllocationsDelegate, this.onGetAllocationsCompletedDelegate, userState);
        }
    }
}
