// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Request
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct RoomList : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static RoomList GetRootAsRoomList(ByteBuffer _bb) { return GetRootAsRoomList(_bb, new RoomList()); }
  public static RoomList GetRootAsRoomList(ByteBuffer _bb, RoomList obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public RoomList __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }


  public static void StartRoomList(FlatBufferBuilder builder) { builder.StartTable(0); }
  public static Offset<FlatBuffer.Request.RoomList> EndRoomList(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Request.RoomList>(o);
  }
};


}