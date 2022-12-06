// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Response
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct Route : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static Route GetRootAsRoute(ByteBuffer _bb) { return GetRootAsRoute(_bb, new Route()); }
  public static Route GetRootAsRoute(ByteBuffer _bb, Route obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Route __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Host { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetHostBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetHostBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetHostArray() { return __p.__vector_as_array<byte>(4); }
  public uint Port { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
  public uint Error { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

  public static Offset<FlatBuffer.Response.Route> CreateRoute(FlatBufferBuilder builder,
      StringOffset hostOffset = default(StringOffset),
      uint port = 0,
      uint error = 0) {
    builder.StartTable(3);
    Route.AddError(builder, error);
    Route.AddPort(builder, port);
    Route.AddHost(builder, hostOffset);
    return Route.EndRoute(builder);
  }

  public static void StartRoute(FlatBufferBuilder builder) { builder.StartTable(3); }
  public static void AddHost(FlatBufferBuilder builder, StringOffset hostOffset) { builder.AddOffset(0, hostOffset.Value, 0); }
  public static void AddPort(FlatBufferBuilder builder, uint port) { builder.AddUint(1, port, 0); }
  public static void AddError(FlatBufferBuilder builder, uint error) { builder.AddUint(2, error, 0); }
  public static Offset<FlatBuffer.Response.Route> EndRoute(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Response.Route>(o);
  }
};


}
