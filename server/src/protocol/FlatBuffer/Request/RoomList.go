// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Request

import (
	flatbuffers "github.com/google/flatbuffers/go"
)

type RoomList struct {
	_tab flatbuffers.Table
}

func GetRootAsRoomList(buf []byte, offset flatbuffers.UOffsetT) *RoomList {
	n := flatbuffers.GetUOffsetT(buf[offset:])
	x := &RoomList{}
	x.Init(buf, n+offset)
	return x
}

func (rcv *RoomList) Init(buf []byte, i flatbuffers.UOffsetT) {
	rcv._tab.Bytes = buf
	rcv._tab.Pos = i
}

func (rcv *RoomList) Table() flatbuffers.Table {
	return rcv._tab
}

func RoomListStart(builder *flatbuffers.Builder) {
	builder.StartObject(0)
}
func RoomListEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
