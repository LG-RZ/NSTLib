namespace NSTLib.igStructures.Sg
{
    // Incomplete
    public class CStandardMaterial : igFxMaterial
    {
        [igField(104, typeof(igFloatMetaField))] public float _normalMapScale = 1f;
        [igField(108, typeof(igBooleanMetaField))] public bool _parallaxMapping = false;
        [igField(109, typeof(igBooleanMetaField))] public bool _additive = false;
        [igField(110, typeof(igUnsignedShortMetaField))] public ushort _numSteps;
        [igField(112, typeof(igVec3fMetaField))] public igVec3f _parallaxStrength = igVec3f.One;
        [igField(124, typeof(igBooleanMetaField))] public bool _useVertAlphaAsHeight;
        [igField(125, typeof(igBooleanMetaField))] public bool _useVertColorAsHeight;
        [igField(126, typeof(igBooleanMetaField))] public bool _flipVertAlphaHeight;
        [igField(127, typeof(igBooleanMetaField))] public bool _parallaxDepth;
        [igField(128, typeof(igVec3fMetaField))] public igVec3f _parallaxParams = igVec3f.One;
        [igField(144, typeof(igIntMetaField))] public int _textureBitfield;
        [igField(148, typeof(igBooleanMetaField))] public bool _hasEmissiveMap;
        [igField(149, typeof(igBooleanMetaField))] public bool _hasNormalMap;
        [igField(150, typeof(igBooleanMetaField))] public bool _mobileUseNormalMap;

        #region Textures

        [igField(152, typeof(igStringMetaField))] public string _textureName_diffuse = @"textures:\default_c.png";
        [igField(156, typeof(igUnsignedCharMetaField))] public byte _textureCompression_diffuse;
        [igField(157, typeof(igUnsignedCharMetaField))] public byte _textureMips_diffuse;
        [igField(158, typeof(igUnsignedCharMetaField))] public byte _textureAllowDownsample_diffuse;

        [igField(160, typeof(igStringMetaField))] public string _textureName_normal = @"textures:\default_n.png";
        [igField(164, typeof(igUnsignedCharMetaField))] public byte _textureCompression_normal;
        [igField(165, typeof(igUnsignedCharMetaField))] public byte _textureMips_normal;
        [igField(166, typeof(igUnsignedCharMetaField))] public byte _textureAllowDownsample_normal;

        [igField(168, typeof(igStringMetaField))] public string _textureName_gloss = @"textures:\default_g.png";
        [igField(172, typeof(igUnsignedCharMetaField))] public byte _textureCompression_gloss;
        [igField(173, typeof(igUnsignedCharMetaField))] public byte _textureMips_gloss;
        [igField(174, typeof(igUnsignedCharMetaField))] public byte _textureAllowDownsample_gloss;

        [igField(176, typeof(igStringMetaField))] public string _textureName_metal = @"textures:\default_m.png";
        [igField(180, typeof(igUnsignedCharMetaField))] public byte _textureCompression_metal;
        [igField(181, typeof(igUnsignedCharMetaField))] public byte _textureMips_metal;
        [igField(182, typeof(igUnsignedCharMetaField))] public byte _textureAllowDownsample_metal;

        [igField(184, typeof(igStringMetaField))] public string _textureName_emissive = @"textures:\default_e.png";
        [igField(188, typeof(igUnsignedCharMetaField))] public byte _textureCompression_emissive;
        [igField(189, typeof(igUnsignedCharMetaField))] public byte _textureMips_emissive;
        [igField(190, typeof(igUnsignedCharMetaField))] public byte _textureAllowDownsample_emissive;

        [igField(192, typeof(igStringMetaField))] public string _textureName_backscatter = @"textures:\default_b.png";
        [igField(196, typeof(igUnsignedCharMetaField))] public byte _textureCompression_backscatter;
        [igField(197, typeof(igUnsignedCharMetaField))] public byte _textureMips_backscatter;
        [igField(198, typeof(igUnsignedCharMetaField))] public byte _textureAllowDownsample_backscatter;

        [igField(200, typeof(igStringMetaField))] public string _textureName_height = @"textures:\default_h.png";
        [igField(204, typeof(igUnsignedCharMetaField))] public byte _textureCompression_height;
        [igField(205, typeof(igUnsignedCharMetaField))] public byte _textureMips_height;
        [igField(206, typeof(igUnsignedCharMetaField))] public byte _textureAllowDownsample_height;

        #endregion

        [igField(208, typeof(igMatrix44fMetaField))] public igMatrix44f _textureTransform = igMatrix44f.Identity;
        [igField(272, typeof(igVec4fMetaField))] public igVec4f _color = igVec4f.One;
        [igField(320, typeof(igBooleanMetaField))] public bool _vertexWibbleEnabled;
    }
}
