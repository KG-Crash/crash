// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Response

import (
	flatbuffers "github.com/google/flatbuffers/go"
)

type KickedRoom struct {
	_tab flatbuffers.Table
}

func GetRootAsKickedRoom(buf []byte, offset flatbuffers.UOffsetT) *KickedRoom {
	n := flatbuffers.GetUOffsetT(buf[offset:])
	x := &KickedRoom{}
	x.Init(buf, n+offset)
	return x
}

func (rcv *KickedRoom) Init(buf []byte, i flatbuffers.UOffsetT) {
	rcv._tab.Bytes = buf
	rcv._tab.Pos = i
}

func (rcv *KickedRoom) Table() flatbuffers.Table {
	return rcv._tab
}

func KickedRoomStart(builder *flatbuffers.Builder) {
	builder.StartObject(0)
}
func KickedRoomEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
