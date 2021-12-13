// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Response
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct Action : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static Action GetRootAsAction(ByteBuffer _bb) { return GetRootAsAction(_bb, new Action()); }
  public static Action GetRootAsAction(ByteBuffer _bb, Action obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Action __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int Frame { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int Id { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public uint Param1 { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
  public uint Param2 { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

  public static Offset<FlatBuffer.Response.Action> CreateAction(FlatBufferBuilder builder,
      int frame = 0,
      int id = 0,
      uint param1 = 0,
      uint param2 = 0) {
    builder.StartTable(4);
    Action.AddParam2(builder, param2);
    Action.AddParam1(builder, param1);
    Action.AddId(builder, id);
    Action.AddFrame(builder, frame);
    return Action.EndAction(builder);
  }

  public static void StartAction(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddFrame(FlatBufferBuilder builder, int frame) { builder.AddInt(0, frame, 0); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(1, id, 0); }
  public static void AddParam1(FlatBufferBuilder builder, uint param1) { builder.AddUint(2, param1, 0); }
  public static void AddParam2(FlatBufferBuilder builder, uint param2) { builder.AddUint(3, param2, 0); }
  public static Offset<FlatBuffer.Response.Action> EndAction(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Response.Action>(o);
  }
};


}
