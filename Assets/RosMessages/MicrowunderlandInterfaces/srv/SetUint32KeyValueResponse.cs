//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.MicrowunderlandInterfaces
{
    [Serializable]
    public class SetUint32KeyValueResponse : Message
    {
        public const string k_RosMessageName = "microwunderland_interfaces/SetUint32KeyValue";
        public override string RosMessageName => k_RosMessageName;

        public byte status;
        //  status code

        public SetUint32KeyValueResponse()
        {
            this.status = 0;
        }

        public SetUint32KeyValueResponse(byte status)
        {
            this.status = status;
        }

        public static SetUint32KeyValueResponse Deserialize(MessageDeserializer deserializer) => new SetUint32KeyValueResponse(deserializer);

        private SetUint32KeyValueResponse(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.status);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.status);
        }

        public override string ToString()
        {
            return "SetUint32KeyValueResponse: " +
            "\nstatus: " + status.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Response);
        }
    }
}
