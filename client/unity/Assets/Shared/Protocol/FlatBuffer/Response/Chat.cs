// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Response
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct Chat : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static Chat GetRootAsChat(ByteBuffer _bb) { return GetRootAsChat(_bb, new Chat()); }
  public static Chat GetRootAsChat(ByteBuffer _bb, Chat obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Chat __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string User { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetUserBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetUserBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetUserArray() { return __p.__vector_as_array<byte>(4); }
  public string Message { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetMessageBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
  public ArraySegment<byte>? GetMessageBytes() { return __p.__vector_as_arraysegment(6); }
#endif
  public byte[] GetMessageArray() { return __p.__vector_as_array<byte>(6); }
  public uint Error { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

  public static Offset<FlatBuffer.Response.Chat> CreateChat(FlatBufferBuilder builder,
      StringOffset userOffset = default(StringOffset),
      StringOffset messageOffset = default(StringOffset),
      uint error = 0) {
    builder.StartTable(3);
    Chat.AddError(builder, error);
    Chat.AddMessage(builder, messageOffset);
    Chat.AddUser(builder, userOffset);
    return Chat.EndChat(builder);
  }

  public static void StartChat(FlatBufferBuilder builder) { builder.StartTable(3); }
  public static void AddUser(FlatBufferBuilder builder, StringOffset userOffset) { builder.AddOffset(0, userOffset.Value, 0); }
  public static void AddMessage(FlatBufferBuilder builder, StringOffset messageOffset) { builder.AddOffset(1, messageOffset.Value, 0); }
  public static void AddError(FlatBufferBuilder builder, uint error) { builder.AddUint(2, error, 0); }
  public static Offset<FlatBuffer.Response.Chat> EndChat(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Response.Chat>(o);
  }
};


}
