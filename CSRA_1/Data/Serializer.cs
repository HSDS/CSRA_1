using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSRA_1.Data
{
    public class Serializer
    {

        public static bool BinarySerialize(string FilePath, Object ObjectToSerialize)
        {
            bool status = true;
            if (Directory.Exists(Path.GetDirectoryName(FilePath)) && ObjectToSerialize != null)
            {
                try
                {
                    System.IO.FileStream fs = new System.IO.FileStream(FilePath, System.IO.FileMode.Create);
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bf.Serialize(fs, ObjectToSerialize);
                    fs.Close();
                }
                catch
                {
                    status = false;
                }
            }
            else
            {
                status = false;
            }
            return status;
        }

        public static object BinaryDeserialize(string FilePath)
        {
            object deserializedObject = null;
            if (System.IO.File.Exists(FilePath))
            {
                System.IO.FileStream fs = new System.IO.FileStream(FilePath, System.IO.FileMode.Open);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                deserializedObject = bf.Deserialize(fs);
                fs.Close();
            }
            return deserializedObject;
        }

        public static bool XmlSerialize(string FilePath, Object ObjectToSerialize)
        {
            bool status = true;
            if (Directory.Exists(Path.GetDirectoryName(FilePath)) && ObjectToSerialize != null)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(ObjectToSerialize.GetType());
                    Stream stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    serializer.Serialize(stream, ObjectToSerialize);
                    stream.Close();
                }
                catch
                {
                    status = false;
                }
            }
            else
            {
                status = false;
            }
            return status;
        }

        public static bool XmlSerialize(MemoryStream stream, Object ObjectToSerialize)
        {
            bool status = true;
            if (ObjectToSerialize != null)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(ObjectToSerialize.GetType());
                    serializer.Serialize(stream, ObjectToSerialize);
                }
                catch
                {
                    status = false;
                }
            }
            else
            {
                status = false;
            }
            return status;

        }

        public static object XmlDeserialize(string FilePath, System.Type Type)
        {
            object deserializedObject = null;
            try
            {
                if (System.IO.File.Exists(FilePath))
                {
                    System.IO.FileStream fs = new System.IO.FileStream(FilePath, System.IO.FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(Type);
                    deserializedObject = serializer.Deserialize(fs);
                    fs.Close();
                }
            }
            catch
            {
                //  just return null
            }
            return deserializedObject;
        }

        public static object XmlDeserialize(MemoryStream stream, System.Type Type)
        {
            object deserializedObject = null;
            try
            {
                if (stream != null)
                {
                    XmlSerializer serializer = new XmlSerializer(Type);
                    deserializedObject = serializer.Deserialize(stream);
                }
            }
            catch
            {
                deserializedObject = null;
            }
            return deserializedObject;
        }

        public static object DeepCopy(object objectToCopy)
        {
            object newObject = null;

            try
            {
                MemoryStream stream = new MemoryStream();
                if (XmlSerialize(stream, objectToCopy))
                {
                    // rewind stream
                    stream.Position = 0;
                    newObject = XmlDeserialize(stream, objectToCopy.GetType());
                }
                stream.Close();
            }
            catch
            {
                newObject = null;
            }

            return newObject;
        }

        public static object XmlDeserialize(Stream stream, System.Type Type)
        {
            object deserializedObject = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(Type);
                deserializedObject = serializer.Deserialize(stream);
            }
            catch
            {

            }

            return deserializedObject;
        }


    }
}
