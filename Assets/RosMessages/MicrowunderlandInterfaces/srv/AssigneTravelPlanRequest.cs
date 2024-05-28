//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.MicrowunderlandInterfaces
{
    [Serializable]
    public class AssigneTravelPlanRequest : Message
    {
        public const string k_RosMessageName = "microwunderland_interfaces/AssigneTravelPlan";
        public override string RosMessageName => k_RosMessageName;

        //  assigns a travel plan to a tracked vehicle
        public string plan_name;
        //  name of the travel plan
        public string tracker_id;
        //  tracking id of the vehicle
        public string ipadress;
        //  ip address of the tinycar

        public AssigneTravelPlanRequest()
        {
            this.plan_name = "";
            this.tracker_id = "";
            this.ipadress = "";
        }

        public AssigneTravelPlanRequest(string plan_name, string tracker_id, string ipadress)
        {
            this.plan_name = plan_name;
            this.tracker_id = tracker_id;
            this.ipadress = ipadress;
        }

        public static AssigneTravelPlanRequest Deserialize(MessageDeserializer deserializer) => new AssigneTravelPlanRequest(deserializer);

        private AssigneTravelPlanRequest(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.plan_name);
            deserializer.Read(out this.tracker_id);
            deserializer.Read(out this.ipadress);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.plan_name);
            serializer.Write(this.tracker_id);
            serializer.Write(this.ipadress);
        }

        public override string ToString()
        {
            return "AssigneTravelPlanRequest: " +
            "\nplan_name: " + plan_name.ToString() +
            "\ntracker_id: " + tracker_id.ToString() +
            "\nipadress: " + ipadress.ToString();
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
