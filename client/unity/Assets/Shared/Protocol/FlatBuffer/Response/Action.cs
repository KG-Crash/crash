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

  public string User { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetUserBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetUserBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetUserArray() { return __p.__vector_as_array<byte>(4); }
  public int Frame { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int Id { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int PositionX { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int PositionY { get { int o = __p.__offset(12); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }

  public static Offset<FlatBuffer.Response.Action> CreateAction(FlatBufferBuilder builder,
      StringOffset userOffset = default(StringOffset),
      int frame = 0,
      int id = 0,
      int positionX = 0,
      int positionY = 0) {
    builder.StartTable(5);
    Action.AddPositionY(builder, positionY);
    Action.AddPositionX(builder, positionX);
    Action.AddId(builder, id);
    Action.AddFrame(builder, frame);
    Action.AddUser(builder, userOffset);
    return Action.EndAction(builder);
  }

  public static void StartAction(FlatBufferBuilder builder) { builder.StartTable(5); }
  public static void AddUser(FlatBufferBuilder builder, StringOffset userOffset) { builder.AddOffset(0, userOffset.Value, 0); }
  public static void AddFrame(FlatBufferBuilder builder, int frame) { builder.AddInt(1, frame, 0); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(2, id, 0); }
  public static void AddPositionX(FlatBufferBuilder builder, int positionX) { builder.AddInt(3, positionX, 0); }
  public static void AddPositionY(FlatBufferBuilder builder, int positionY) { builder.AddInt(4, positionY, 0); }
  public static Offset<FlatBuffer.Response.Action> EndAction(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Response.Action>(o);
  }
};


}
