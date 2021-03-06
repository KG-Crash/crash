// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Response
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct KickRoom : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static KickRoom GetRootAsKickRoom(ByteBuffer _bb) { return GetRootAsKickRoom(_bb, new KickRoom()); }
  public static KickRoom GetRootAsKickRoom(ByteBuffer _bb, KickRoom obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public KickRoom __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public bool Success { get { int o = __p.__offset(4); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public uint Error { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

  public static Offset<FlatBuffer.Response.KickRoom> CreateKickRoom(FlatBufferBuilder builder,
      bool success = false,
      uint error = 0) {
    builder.StartTable(2);
    KickRoom.AddError(builder, error);
    KickRoom.AddSuccess(builder, success);
    return KickRoom.EndKickRoom(builder);
  }

  public static void StartKickRoom(FlatBufferBuilder builder) { builder.StartTable(2); }
  public static void AddSuccess(FlatBufferBuilder builder, bool success) { builder.AddBool(0, success, false); }
  public static void AddError(FlatBufferBuilder builder, uint error) { builder.AddUint(1, error, 0); }
  public static Offset<FlatBuffer.Response.KickRoom> EndKickRoom(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Response.KickRoom>(o);
  }
};


}
