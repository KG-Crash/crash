// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Request
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct RouteEnter : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static RouteEnter GetRootAsRouteEnter(ByteBuffer _bb) { return GetRootAsRouteEnter(_bb, new RouteEnter()); }
  public static RouteEnter GetRootAsRouteEnter(ByteBuffer _bb, RouteEnter obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public RouteEnter __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Id { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetIdArray() { return __p.__vector_as_array<byte>(4); }

  public static Offset<FlatBuffer.Request.RouteEnter> CreateRouteEnter(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset)) {
    builder.StartTable(1);
    RouteEnter.AddId(builder, idOffset);
    return RouteEnter.EndRouteEnter(builder);
  }

  public static void StartRouteEnter(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static Offset<FlatBuffer.Request.RouteEnter> EndRouteEnter(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Request.RouteEnter>(o);
  }
};


}