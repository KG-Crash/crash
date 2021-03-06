// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Response

import (
	flatbuffers "github.com/google/flatbuffers/go"
)

type KickRoom struct {
	_tab flatbuffers.Table
}

func GetRootAsKickRoom(buf []byte, offset flatbuffers.UOffsetT) *KickRoom {
	n := flatbuffers.GetUOffsetT(buf[offset:])
	x := &KickRoom{}
	x.Init(buf, n+offset)
	return x
}

func (rcv *KickRoom) Init(buf []byte, i flatbuffers.UOffsetT) {
	rcv._tab.Bytes = buf
	rcv._tab.Pos = i
}

func (rcv *KickRoom) Table() flatbuffers.Table {
	return rcv._tab
}

func (rcv *KickRoom) Success() bool {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(4))
	if o != 0 {
		return rcv._tab.GetBool(o + rcv._tab.Pos)
	}
	return false
}

func (rcv *KickRoom) MutateSuccess(n bool) bool {
	return rcv._tab.MutateBoolSlot(4, n)
}

func (rcv *KickRoom) Error() uint32 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(6))
	if o != 0 {
		return rcv._tab.GetUint32(o + rcv._tab.Pos)
	}
	return 0
}

func (rcv *KickRoom) MutateError(n uint32) bool {
	return rcv._tab.MutateUint32Slot(6, n)
}

func KickRoomStart(builder *flatbuffers.Builder) {
	builder.StartObject(2)
}
func KickRoomAddSuccess(builder *flatbuffers.Builder, success bool) {
	builder.PrependBoolSlot(0, success, false)
}
func KickRoomAddError(builder *flatbuffers.Builder, error uint32) {
	builder.PrependUint32Slot(1, error, 0)
}
func KickRoomEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
