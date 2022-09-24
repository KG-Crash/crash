// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Response
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct ActionQueue : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static ActionQueue GetRootAsActionQueue(ByteBuffer _bb) { return GetRootAsActionQueue(_bb, new ActionQueue()); }
  public static ActionQueue GetRootAsActionQueue(ByteBuffer _bb, ActionQueue obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public ActionQueue __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int Sequence { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public FlatBuffer.Response.Action? Actions(int j) { int o = __p.__offset(6); return o != 0 ? (FlatBuffer.Response.Action?)(new FlatBuffer.Response.Action()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int ActionsLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
  public int Turn { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public uint Error { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }

  public static Offset<FlatBuffer.Response.ActionQueue> CreateActionQueue(FlatBufferBuilder builder,
      int sequence = 0,
      VectorOffset actionsOffset = default(VectorOffset),
      int turn = 0,
      uint error = 0) {
    builder.StartTable(4);
    ActionQueue.AddError(builder, error);
    ActionQueue.AddTurn(builder, turn);
    ActionQueue.AddActions(builder, actionsOffset);
    ActionQueue.AddSequence(builder, sequence);
    return ActionQueue.EndActionQueue(builder);
  }

  public static void StartActionQueue(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddSequence(FlatBufferBuilder builder, int sequence) { builder.AddInt(0, sequence, 0); }
  public static void AddActions(FlatBufferBuilder builder, VectorOffset actionsOffset) { builder.AddOffset(1, actionsOffset.Value, 0); }
  public static VectorOffset CreateActionsVector(FlatBufferBuilder builder, Offset<FlatBuffer.Response.Action>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateActionsVectorBlock(FlatBufferBuilder builder, Offset<FlatBuffer.Response.Action>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static void StartActionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTurn(FlatBufferBuilder builder, int turn) { builder.AddInt(2, turn, 0); }
  public static void AddError(FlatBufferBuilder builder, uint error) { builder.AddUint(3, error, 0); }
  public static Offset<FlatBuffer.Response.ActionQueue> EndActionQueue(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Response.ActionQueue>(o);
  }
};


}
