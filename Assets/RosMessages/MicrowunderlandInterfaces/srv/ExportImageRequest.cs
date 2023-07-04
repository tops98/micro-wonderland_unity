//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.MicrowunderlandInterfaces
{
    [Serializable]
    public class ExportImageRequest : Message
    {
        public const string k_RosMessageName = "microwunderland_interfaces/ExportImage";
        public override string RosMessageName => k_RosMessageName;

        //  Allows to export and save compressed image_msg to local storage of the service node
        public string export_dir;
        //  path to thr
        public string topic_name;
        //  name of the topic to record
        public ushort interval_ms;
        //  zero means the param will not be used
        public ushort nth_frame;
        //  zero means the param will not be used
        public uint max_time_ms;
        //  if zero no max time will be used
        public ushort max_frames;
        //  if zero no frame limit will be used

        public ExportImageRequest()
        {
            this.export_dir = "";
            this.topic_name = "";
            this.interval_ms = 0;
            this.nth_frame = 0;
            this.max_time_ms = 0;
            this.max_frames = 0;
        }

        public ExportImageRequest(string export_dir, string topic_name, ushort interval_ms, ushort nth_frame, uint max_time_ms, ushort max_frames)
        {
            this.export_dir = export_dir;
            this.topic_name = topic_name;
            this.interval_ms = interval_ms;
            this.nth_frame = nth_frame;
            this.max_time_ms = max_time_ms;
            this.max_frames = max_frames;
        }

        public static ExportImageRequest Deserialize(MessageDeserializer deserializer) => new ExportImageRequest(deserializer);

        private ExportImageRequest(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.export_dir);
            deserializer.Read(out this.topic_name);
            deserializer.Read(out this.interval_ms);
            deserializer.Read(out this.nth_frame);
            deserializer.Read(out this.max_time_ms);
            deserializer.Read(out this.max_frames);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.export_dir);
            serializer.Write(this.topic_name);
            serializer.Write(this.interval_ms);
            serializer.Write(this.nth_frame);
            serializer.Write(this.max_time_ms);
            serializer.Write(this.max_frames);
        }

        public override string ToString()
        {
            return "ExportImageRequest: " +
            "\nexport_dir: " + export_dir.ToString() +
            "\ntopic_name: " + topic_name.ToString() +
            "\ninterval_ms: " + interval_ms.ToString() +
            "\nnth_frame: " + nth_frame.ToString() +
            "\nmax_time_ms: " + max_time_ms.ToString() +
            "\nmax_frames: " + max_frames.ToString();
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
