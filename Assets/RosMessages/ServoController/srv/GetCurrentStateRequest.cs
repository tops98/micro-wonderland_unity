//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.ServoController
{
    [Serializable]
    public class GetCurrentStateRequest : Message
    {
        public const string k_RosMessageName = "servo_controller/GetCurrentState";
        public override string RosMessageName => k_RosMessageName;

        //  returns the currently active state of a switch
        public string switch_name;
        //  name of the switch that shall be controlled

        public GetCurrentStateRequest()
        {
            this.switch_name = "";
        }

        public GetCurrentStateRequest(string switch_name)
        {
            this.switch_name = switch_name;
        }

        public static GetCurrentStateRequest Deserialize(MessageDeserializer deserializer) => new GetCurrentStateRequest(deserializer);

        private GetCurrentStateRequest(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.switch_name);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.switch_name);
        }

        public override string ToString()
        {
            return "GetCurrentStateRequest: " +
            "\nswitch_name: " + switch_name.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
