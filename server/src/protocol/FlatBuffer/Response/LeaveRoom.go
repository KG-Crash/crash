// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Response

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

func (rcv *LeaveRoom) User() []byte {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(4))
	if o != 0 {
		return rcv._tab.ByteVector(o + rcv._tab.Pos)
	}
	return nil
}

func (rcv *LeaveRoom) NewMaster() []byte {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(6))
	if o != 0 {
		return rcv._tab.ByteVector(o + rcv._tab.Pos)
	}
	return nil
}

func (rcv *LeaveRoom) Error() uint32 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(8))
	if o != 0 {
		return rcv._tab.GetUint32(o + rcv._tab.Pos)
	}
	return 0
}

func (rcv *LeaveRoom) MutateError(n uint32) bool {
	return rcv._tab.MutateUint32Slot(8, n)
}

func LeaveRoomStart(builder *flatbuffers.Builder) {
	builder.StartObject(3)
}
func LeaveRoomAddUser(builder *flatbuffers.Builder, user flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(0, flatbuffers.UOffsetT(user), 0)
}
func LeaveRoomAddNewMaster(builder *flatbuffers.Builder, newMaster flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(1, flatbuffers.UOffsetT(newMaster), 0)
}
func LeaveRoomAddError(builder *flatbuffers.Builder, error uint32) {
	builder.PrependUint32Slot(2, error, 0)
}
func LeaveRoomEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
