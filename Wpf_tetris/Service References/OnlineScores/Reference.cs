﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wpf_tetris.OnlineScores {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ArrayOfString", Namespace="http://tempuri.org/", ItemName="string")]
    [System.SerializableAttribute()]
    public class ArrayOfString : System.Collections.Generic.List<string> {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="OnlineScores.OnlineScoresSoap")]
    public interface OnlineScoresSoap {
        
        // CODEGEN: Generating message contract since element name GetScoresResult from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetScores", ReplyAction="*")]
        Wpf_tetris.OnlineScores.GetScoresResponse GetScores(Wpf_tetris.OnlineScores.GetScoresRequest request);
        
        // CODEGEN: Generating message contract since element name name from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/PutScore", ReplyAction="*")]
        Wpf_tetris.OnlineScores.PutScoreResponse PutScore(Wpf_tetris.OnlineScores.PutScoreRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetScoresRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetScores", Namespace="http://tempuri.org/", Order=0)]
        public Wpf_tetris.OnlineScores.GetScoresRequestBody Body;
        
        public GetScoresRequest() {
        }
        
        public GetScoresRequest(Wpf_tetris.OnlineScores.GetScoresRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class GetScoresRequestBody {
        
        public GetScoresRequestBody() {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetScoresResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetScoresResponse", Namespace="http://tempuri.org/", Order=0)]
        public Wpf_tetris.OnlineScores.GetScoresResponseBody Body;
        
        public GetScoresResponse() {
        }
        
        public GetScoresResponse(Wpf_tetris.OnlineScores.GetScoresResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetScoresResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public Wpf_tetris.OnlineScores.ArrayOfString GetScoresResult;
        
        public GetScoresResponseBody() {
        }
        
        public GetScoresResponseBody(Wpf_tetris.OnlineScores.ArrayOfString GetScoresResult) {
            this.GetScoresResult = GetScoresResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PutScoreRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="PutScore", Namespace="http://tempuri.org/", Order=0)]
        public Wpf_tetris.OnlineScores.PutScoreRequestBody Body;
        
        public PutScoreRequest() {
        }
        
        public PutScoreRequest(Wpf_tetris.OnlineScores.PutScoreRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class PutScoreRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string name;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public int level;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=2)]
        public int score;
        
        public PutScoreRequestBody() {
        }
        
        public PutScoreRequestBody(string name, int level, int score) {
            this.name = name;
            this.level = level;
            this.score = score;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PutScoreResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="PutScoreResponse", Namespace="http://tempuri.org/", Order=0)]
        public Wpf_tetris.OnlineScores.PutScoreResponseBody Body;
        
        public PutScoreResponse() {
        }
        
        public PutScoreResponse(Wpf_tetris.OnlineScores.PutScoreResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class PutScoreResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public bool PutScoreResult;
        
        public PutScoreResponseBody() {
        }
        
        public PutScoreResponseBody(bool PutScoreResult) {
            this.PutScoreResult = PutScoreResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface OnlineScoresSoapChannel : Wpf_tetris.OnlineScores.OnlineScoresSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OnlineScoresSoapClient : System.ServiceModel.ClientBase<Wpf_tetris.OnlineScores.OnlineScoresSoap>, Wpf_tetris.OnlineScores.OnlineScoresSoap {
        
        public OnlineScoresSoapClient() {
        }
        
        public OnlineScoresSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public OnlineScoresSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OnlineScoresSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OnlineScoresSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Wpf_tetris.OnlineScores.GetScoresResponse Wpf_tetris.OnlineScores.OnlineScoresSoap.GetScores(Wpf_tetris.OnlineScores.GetScoresRequest request) {
            return base.Channel.GetScores(request);
        }
        
        public Wpf_tetris.OnlineScores.ArrayOfString GetScores() {
            Wpf_tetris.OnlineScores.GetScoresRequest inValue = new Wpf_tetris.OnlineScores.GetScoresRequest();
            inValue.Body = new Wpf_tetris.OnlineScores.GetScoresRequestBody();
            Wpf_tetris.OnlineScores.GetScoresResponse retVal = ((Wpf_tetris.OnlineScores.OnlineScoresSoap)(this)).GetScores(inValue);
            return retVal.Body.GetScoresResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Wpf_tetris.OnlineScores.PutScoreResponse Wpf_tetris.OnlineScores.OnlineScoresSoap.PutScore(Wpf_tetris.OnlineScores.PutScoreRequest request) {
            return base.Channel.PutScore(request);
        }
        
        public bool PutScore(string name, int level, int score) {
            Wpf_tetris.OnlineScores.PutScoreRequest inValue = new Wpf_tetris.OnlineScores.PutScoreRequest();
            inValue.Body = new Wpf_tetris.OnlineScores.PutScoreRequestBody();
            inValue.Body.name = name;
            inValue.Body.level = level;
            inValue.Body.score = score;
            Wpf_tetris.OnlineScores.PutScoreResponse retVal = ((Wpf_tetris.OnlineScores.OnlineScoresSoap)(this)).PutScore(inValue);
            return retVal.Body.PutScoreResult;
        }
    }
}
