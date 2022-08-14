// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Response

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

func (rcv *RoomList) Rooms(obj *Room, j int) bool {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(4))
	if o != 0 {
		x := rcv._tab.Vector(o)
		x += flatbuffers.UOffsetT(j) * 4
		x = rcv._tab.Indirect(x)
		obj.Init(rcv._tab.Bytes, x)
		return true
	}
	return false
}

func (rcv *RoomList) RoomsLength() int {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(4))
	if o != 0 {
		return rcv._tab.VectorLen(o)
	}
	return 0
}

func (rcv *RoomList) Error() uint32 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(6))
	if o != 0 {
		return rcv._tab.GetUint32(o + rcv._tab.Pos)
	}
	return 0
}

func (rcv *RoomList) MutateError(n uint32) bool {
	return rcv._tab.MutateUint32Slot(6, n)
}

func RoomListStart(builder *flatbuffers.Builder) {
	builder.StartObject(2)
}
func RoomListAddRooms(builder *flatbuffers.Builder, rooms flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(0, flatbuffers.UOffsetT(rooms), 0)
}
func RoomListStartRoomsVector(builder *flatbuffers.Builder, numElems int) flatbuffers.UOffsetT {
	return builder.StartVector(4, numElems, 4)
}
func RoomListAddError(builder *flatbuffers.Builder, error uint32) {
	builder.PrependUint32Slot(1, error, 0)
}
func RoomListEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
