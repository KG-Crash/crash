// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Response

import (
	flatbuffers "github.com/google/flatbuffers/go"
)

type Chat struct {
	_tab flatbuffers.Table
}

func GetRootAsChat(buf []byte, offset flatbuffers.UOffsetT) *Chat {
	n := flatbuffers.GetUOffsetT(buf[offset:])
	x := &Chat{}
	x.Init(buf, n+offset)
	return x
}

func (rcv *Chat) Init(buf []byte, i flatbuffers.UOffsetT) {
	rcv._tab.Bytes = buf
	rcv._tab.Pos = i
}

func (rcv *Chat) Table() flatbuffers.Table {
	return rcv._tab
}

func (rcv *Chat) User() []byte {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(4))
	if o != 0 {
		return rcv._tab.ByteVector(o + rcv._tab.Pos)
	}
	return nil
}

func (rcv *Chat) Message() []byte {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(6))
	if o != 0 {
		return rcv._tab.ByteVector(o + rcv._tab.Pos)
	}
	return nil
}

func (rcv *Chat) Error() uint32 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(8))
	if o != 0 {
		return rcv._tab.GetUint32(o + rcv._tab.Pos)
	}
	return 0
}

func (rcv *Chat) MutateError(n uint32) bool {
	return rcv._tab.MutateUint32Slot(8, n)
}

func ChatStart(builder *flatbuffers.Builder) {
	builder.StartObject(3)
}
func ChatAddUser(builder *flatbuffers.Builder, user flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(0, flatbuffers.UOffsetT(user), 0)
}
func ChatAddMessage(builder *flatbuffers.Builder, message flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(1, flatbuffers.UOffsetT(message), 0)
}
func ChatAddError(builder *flatbuffers.Builder, error uint32) {
	builder.PrependUint32Slot(2, error, 0)
}
func ChatEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}