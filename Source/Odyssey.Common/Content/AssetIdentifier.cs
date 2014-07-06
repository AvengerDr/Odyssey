﻿using SharpYaml.Serialization;
using System;
using System.ComponentModel;

namespace Odyssey.Content
{
    public class AssetIdentifier
    {
        [YamlMember("name")]
        public string Name { get; set; }

        [YamlMember("path")]
        public string Path { get; set; }

        [YamlMember("type")]
        public string Type { get; set; }

        public AssetIdentifier()
        { }

        public AssetIdentifier(string name, string path, string type)
        {
            Name = name;
            Path = path;
            Type = type;
        }
    }
}