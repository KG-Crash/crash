// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatBuffer.Request
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct GameStart : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static GameStart GetRootAsGameStart(ByteBuffer _bb) { return GetRootAsGameStart(_bb, new GameStart()); }
  public static GameStart GetRootAsGameStart(ByteBuffer _bb, GameStart obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public GameStart __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }


  public static void StartGameStart(FlatBufferBuilder builder) { builder.StartTable(0); }
  public static Offset<FlatBuffer.Request.GameStart> EndGameStart(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<FlatBuffer.Request.GameStart>(o);
  }
};


}
