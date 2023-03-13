// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Request

import (
	flatbuffers "github.com/google/flatbuffers/go"
)

type Login struct {
	_tab flatbuffers.Table
}

func GetRootAsLogin(buf []byte, offset flatbuffers.UOffsetT) *Login {
	n := flatbuffers.GetUOffsetT(buf[offset:])
	x := &Login{}
	x.Init(buf, n+offset)
	return x
}

func (rcv *Login) Init(buf []byte, i flatbuffers.UOffsetT) {
	rcv._tab.Bytes = buf
	rcv._tab.Pos = i
}

func (rcv *Login) Table() flatbuffers.Table {
	return rcv._tab
}

func (rcv *Login) Id() []byte {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(4))
	if o != 0 {
		return rcv._tab.ByteVector(o + rcv._tab.Pos)
	}
	return nil
}

func (rcv *Login) Error() uint32 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(6))
	if o != 0 {
		return rcv._tab.GetUint32(o + rcv._tab.Pos)
	}
	return 0
}

func (rcv *Login) MutateError(n uint32) bool {
	return rcv._tab.MutateUint32Slot(6, n)
}

func LoginStart(builder *flatbuffers.Builder) {
	builder.StartObject(2)
}
func LoginAddId(builder *flatbuffers.Builder, id flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(0, flatbuffers.UOffsetT(id), 0)
}
func LoginAddError(builder *flatbuffers.Builder, error uint32) {
	builder.PrependUint32Slot(1, error, 0)
}
func LoginEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}