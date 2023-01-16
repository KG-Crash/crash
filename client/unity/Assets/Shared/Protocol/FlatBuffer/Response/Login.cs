// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Response
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct Login : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static Login GetRootAsLogin(ByteBuffer _bb) { return GetRootAsLogin(_bb, new Login()); }
  public static Login GetRootAsLogin(ByteBuffer _bb, Login obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Login __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public uint Error { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

  public static Offset<FlatBuffer.Response.Login> CreateLogin(FlatBufferBuilder builder,
      uint error = 0) {
    builder.StartTable(1);
    Login.AddError(builder, error);
    return Login.EndLogin(builder);
  }

  public static void StartLogin(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddError(FlatBufferBuilder builder, uint error) { builder.AddUint(0, error, 0); }
  public static Offset<FlatBuffer.Response.Login> EndLogin(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Response.Login>(o);
  }
};


}
