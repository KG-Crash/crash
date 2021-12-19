// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Response

import (
	flatbuffers "github.com/google/flatbuffers/go"
)

type Ready struct {
	_tab flatbuffers.Table
}

func GetRootAsReady(buf []byte, offset flatbuffers.UOffsetT) *Ready {
	n := flatbuffers.GetUOffsetT(buf[offset:])
	x := &Ready{}
	x.Init(buf, n+offset)
	return x
}

func (rcv *Ready) Init(buf []byte, i flatbuffers.UOffsetT) {
	rcv._tab.Bytes = buf
	rcv._tab.Pos = i
}

func (rcv *Ready) Table() flatbuffers.Table {
	return rcv._tab
}

func (rcv *Ready) Seed() int64 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(4))
	if o != 0 {
		return rcv._tab.GetInt64(o + rcv._tab.Pos)
	}
	return 0
}

func (rcv *Ready) MutateSeed(n int64) bool {
	return rcv._tab.MutateInt64Slot(4, n)
}

func (rcv *Ready) Users(obj *User, j int) bool {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(6))
	if o != 0 {
		x := rcv._tab.Vector(o)
		x += flatbuffers.UOffsetT(j) * 4
		x = rcv._tab.Indirect(x)
		obj.Init(rcv._tab.Bytes, x)
		return true
	}
	return false
}

func (rcv *Ready) UsersLength() int {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(6))
	if o != 0 {
		return rcv._tab.VectorLen(o)
	}
	return 0
}

func ReadyStart(builder *flatbuffers.Builder) {
	builder.StartObject(2)
}
func ReadyAddSeed(builder *flatbuffers.Builder, seed int64) {
	builder.PrependInt64Slot(0, seed, 0)
}
func ReadyAddUsers(builder *flatbuffers.Builder, users flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(1, flatbuffers.UOffsetT(users), 0)
}
func ReadyStartUsersVector(builder *flatbuffers.Builder, numElems int) flatbuffers.UOffsetT {
	return builder.StartVector(4, numElems, 4)
}
func ReadyEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
