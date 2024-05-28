//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.MicrowunderlandInterfaces
{
    [Serializable]
    public class GetUInt32Request : Message
    {
        public const string k_RosMessageName = "microwunderland_interfaces/GetUInt32";
        public override string RosMessageName => k_RosMessageName;

        //  service that returns a UInt32 for a given key and an 8bit status code
        public string key;
        //  key

        public GetUInt32Request()
        {
            this.key = "";
        }

        public GetUInt32Request(string key)
        {
            this.key = key;
        }

        public static GetUInt32Request Deserialize(MessageDeserializer deserializer) => new GetUInt32Request(deserializer);

        private GetUInt32Request(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.key);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.key);
        }

        public override string ToString()
        {
            return "GetUInt32Request: " +
            "\nkey: " + key.ToString();
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