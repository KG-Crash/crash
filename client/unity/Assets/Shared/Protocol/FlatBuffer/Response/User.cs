// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Response
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct User : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static User GetRootAsUser(ByteBuffer _bb) { return GetRootAsUser(_bb, new User()); }
  public static User GetRootAsUser(ByteBuffer _bb, User obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public User __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Id { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetIdBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetIdArray() { return __p.__vector_as_array<byte>(4); }
  public int Team { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int Sequence { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }

  public static Offset<FlatBuffer.Response.User> CreateUser(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      int team = 0,
      int sequence = 0) {
    builder.StartTable(3);
    User.AddSequence(builder, sequence);
    User.AddTeam(builder, team);
    User.AddId(builder, idOffset);
    return User.EndUser(builder);
  }

  public static void StartUser(FlatBufferBuilder builder) { builder.StartTable(3); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddTeam(FlatBufferBuilder builder, int team) { builder.AddInt(1, team, 0); }
  public static void AddSequence(FlatBufferBuilder builder, int sequence) { builder.AddInt(2, sequence, 0); }
  public static Offset<FlatBuffer.Response.User> EndUser(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Response.User>(o);
  }
};


}
