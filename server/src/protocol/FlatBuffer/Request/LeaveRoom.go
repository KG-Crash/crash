// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Request

import (
	flatbuffers "github.com/google/flatbuffers/go"
)

type LeaveRoom struct {
	_tab flatbuffers.Table
}

func GetRootAsLeaveRoom(buf []byte, offset flatbuffers.UOffsetT) *LeaveRoom {
	n := flatbuffers.GetUOffsetT(buf[offset:])
	x := &LeaveRoom{}
	x.Init(buf, n+offset)
	return x
}

func (rcv *LeaveRoom) Init(buf []byte, i flatbuffers.UOffsetT) {
	rcv._tab.Bytes = buf
	rcv._tab.Pos = i
}

func (rcv *LeaveRoom) Table() flatbuffers.Table {
	return rcv._tab
}

func LeaveRoomStart(builder *flatbuffers.Builder) {
	builder.StartObject(0)
}
func LeaveRoomEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
