// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Response
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct Resume : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static Resume GetRootAsResume(ByteBuffer _bb) { return GetRootAsResume(_bb, new Resume()); }
  public static Resume GetRootAsResume(ByteBuffer _bb, Resume obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Resume __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string User { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetUserBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetUserBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetUserArray() { return __p.__vector_as_array<byte>(4); }
  public uint Error { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

  public static Offset<FlatBuffer.Response.Resume> CreateResume(FlatBufferBuilder builder,
      StringOffset userOffset = default(StringOffset),
      uint error = 0) {
    builder.StartTable(2);
    Resume.AddError(builder, error);
    Resume.AddUser(builder, userOffset);
    return Resume.EndResume(builder);
  }

  public static void StartResume(FlatBufferBuilder builder) { builder.StartTable(2); }
  public static void AddUser(FlatBufferBuilder builder, StringOffset userOffset) { builder.AddOffset(0, userOffset.Value, 0); }
  public static void AddError(FlatBufferBuilder builder, uint error) { builder.AddUint(1, error, 0); }
  public static Offset<FlatBuffer.Response.Resume> EndResume(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Response.Resume>(o);
  }
};


}
