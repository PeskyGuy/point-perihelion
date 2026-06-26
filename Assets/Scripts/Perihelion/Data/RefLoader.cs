using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using Perihelion.Refs;

namespace Perihelion.Data
{
    public static class RefLoader
    {
        private static readonly Dictionary<Type, XmlSerializer> _serializers = new();

        public static List<T> LoadAll<T>(string directory, string rootElement) where T : RefDef
        {
            var results = new List<T>();
            if (!Directory.Exists(directory)) return results;

            foreach (string file in Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories))
            {
                var loaded = LoadFile<T>(file, rootElement);
                results.AddRange(loaded);
            }
            return results;
        }

        public static List<T> LoadFile<T>(string filePath, string rootElement) where T : RefDef
        {
            var results = new List<T>();
            try
            {
                var doc = new XmlDocument();
                doc.Load(filePath);

                XmlNodeList nodes = doc.DocumentElement?.ChildNodes;
                if (nodes == null) return results;

                var serializer = GetSerializer<T>();

                foreach (XmlNode node in nodes)
                {
                    if (node.NodeType != XmlNodeType.Element) continue;

                    using var reader = new XmlNodeReader(node);
                    if (serializer.CanDeserialize(reader))
                    {
                        reader.MoveToContent();
                        var obj = (T)serializer.Deserialize(reader);
                        results.Add(obj);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[RefLoader] Failed to load {filePath}: {e.Message}");
            }
            return results;
        }

        public static List<PatchRef> LoadPatches(string directory)
        {
            var results = new List<PatchRef>();
            if (!Directory.Exists(directory)) return results;

            var serializer = new XmlSerializer(typeof(PatchRef));

            foreach (string file in Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories))
            {
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(file);

                    XmlNodeList nodes = doc.DocumentElement?.ChildNodes;
                    if (nodes == null) continue;

                    foreach (XmlNode node in nodes)
                    {
                        if (node.NodeType != XmlNodeType.Element) continue;
                        using var reader = new XmlNodeReader(node);
                        if (serializer.CanDeserialize(reader))
                        {
                            reader.MoveToContent();
                            var patch = (PatchRef)serializer.Deserialize(reader);
                            results.Add(patch);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[RefLoader] Failed to load patches from {file}: {e.Message}");
                }
            }
            return results;
        }

        private static XmlSerializer GetSerializer<T>() where T : RefDef
        {
            var type = typeof(T);
            if (!_serializers.TryGetValue(type, out var serializer))
            {
                serializer = new XmlSerializer(type);
                _serializers[type] = serializer;
            }
            return serializer;
        }
    }
}
