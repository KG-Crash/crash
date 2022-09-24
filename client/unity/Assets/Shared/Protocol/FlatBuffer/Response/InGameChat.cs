// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Response
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct InGameChat : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static InGameChat GetRootAsInGameChat(ByteBuffer _bb) { return GetRootAsInGameChat(_bb, new InGameChat()); }
  public static InGameChat GetRootAsInGameChat(ByteBuffer _bb, InGameChat obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public InGameChat __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int Turn { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int Frame { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int Sequence { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public string Message { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetMessageBytes() { return __p.__vector_as_span<byte>(10, 1); }
#else
  public ArraySegment<byte>? GetMessageBytes() { return __p.__vector_as_arraysegment(10); }
#endif
  public byte[] GetMessageArray() { return __p.__vector_as_array<byte>(10); }
  public uint Error { get { int o = __p.__offset(12); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

  public static Offset<FlatBuffer.Response.InGameChat> CreateInGameChat(FlatBufferBuilder builder,
      int turn = 0,
      int frame = 0,
      int sequence = 0,
      StringOffset messageOffset = default(StringOffset),
      uint error = 0) {
    builder.StartTable(5);
    InGameChat.AddError(builder, error);
    InGameChat.AddMessage(builder, messageOffset);
    InGameChat.AddSequence(builder, sequence);
    InGameChat.AddFrame(builder, frame);
    InGameChat.AddTurn(builder, turn);
    return InGameChat.EndInGameChat(builder);
  }

  public static void StartInGameChat(FlatBufferBuilder builder) { builder.StartTable(5); }
  public static void AddTurn(FlatBufferBuilder builder, int turn) { builder.AddInt(0, turn, 0); }
  public static void AddFrame(FlatBufferBuilder builder, int frame) { builder.AddInt(1, frame, 0); }
  public static void AddSequence(FlatBufferBuilder builder, int sequence) { builder.AddInt(2, sequence, 0); }
  public static void AddMessage(FlatBufferBuilder builder, StringOffset messageOffset) { builder.AddOffset(3, messageOffset.Value, 0); }
  public static void AddError(FlatBufferBuilder builder, uint error) { builder.AddUint(4, error, 0); }
  public static Offset<FlatBuffer.Response.InGameChat> EndInGameChat(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Response.InGameChat>(o);
  }
};


}
