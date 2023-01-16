// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Request

import (
	flatbuffers "github.com/google/flatbuffers/go"
)

type CreateRoom struct {
	_tab flatbuffers.Table
}

func GetRootAsCreateRoom(buf []byte, offset flatbuffers.UOffsetT) *CreateRoom {
	n := flatbuffers.GetUOffsetT(buf[offset:])
	x := &CreateRoom{}
	x.Init(buf, n+offset)
	return x
}

func (rcv *CreateRoom) Init(buf []byte, i flatbuffers.UOffsetT) {
	rcv._tab.Bytes = buf
	rcv._tab.Pos = i
}

func (rcv *CreateRoom) Table() flatbuffers.Table {
	return rcv._tab
}

func (rcv *CreateRoom) Id() []byte {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(4))
	if o != 0 {
		return rcv._tab.ByteVector(o + rcv._tab.Pos)
	}
	return nil
}

func (rcv *CreateRoom) Title() []byte {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(6))
	if o != 0 {
		return rcv._tab.ByteVector(o + rcv._tab.Pos)
	}
	return nil
}

func (rcv *CreateRoom) Teams(j int) int32 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(8))
	if o != 0 {
		a := rcv._tab.Vector(o)
		return rcv._tab.GetInt32(a + flatbuffers.UOffsetT(j*4))
	}
	return 0
}

func (rcv *CreateRoom) TeamsLength() int {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(8))
	if o != 0 {
		return rcv._tab.VectorLen(o)
	}
	return 0
}

func (rcv *CreateRoom) MutateTeams(j int, n int32) bool {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(8))
	if o != 0 {
		a := rcv._tab.Vector(o)
		return rcv._tab.MutateInt32(a+flatbuffers.UOffsetT(j*4), n)
	}
	return false
}

func CreateRoomStart(builder *flatbuffers.Builder) {
	builder.StartObject(3)
}
func CreateRoomAddId(builder *flatbuffers.Builder, id flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(0, flatbuffers.UOffsetT(id), 0)
}
func CreateRoomAddTitle(builder *flatbuffers.Builder, title flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(1, flatbuffers.UOffsetT(title), 0)
}
func CreateRoomAddTeams(builder *flatbuffers.Builder, teams flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(2, flatbuffers.UOffsetT(teams), 0)
}
func CreateRoomStartTeamsVector(builder *flatbuffers.Builder, numElems int) flatbuffers.UOffsetT {
	return builder.StartVector(4, numElems, 4)
}
func CreateRoomEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
