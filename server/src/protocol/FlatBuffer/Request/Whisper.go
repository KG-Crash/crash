// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Request

import (
	flatbuffers "github.com/google/flatbuffers/go"
)

type Whisper struct {
	_tab flatbuffers.Table
}

func GetRootAsWhisper(buf []byte, offset flatbuffers.UOffsetT) *Whisper {
	n := flatbuffers.GetUOffsetT(buf[offset:])
	x := &Whisper{}
	x.Init(buf, n+offset)
	return x
}

func (rcv *Whisper) Init(buf []byte, i flatbuffers.UOffsetT) {
	rcv._tab.Bytes = buf
	rcv._tab.Pos = i
}

func (rcv *Whisper) Table() flatbuffers.Table {
	return rcv._tab
}

func (rcv *Whisper) User() []byte {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(4))
	if o != 0 {
		return rcv._tab.ByteVector(o + rcv._tab.Pos)
	}
	return nil
}

func (rcv *Whisper) Message() []byte {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(6))
	if o != 0 {
		return rcv._tab.ByteVector(o + rcv._tab.Pos)
	}
	return nil
}

func WhisperStart(builder *flatbuffers.Builder) {
	builder.StartObject(2)
}
func WhisperAddUser(builder *flatbuffers.Builder, user flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(0, flatbuffers.UOffsetT(user), 0)
}
func WhisperAddMessage(builder *flatbuffers.Builder, message flatbuffers.UOffsetT) {
	builder.PrependUOffsetTSlot(1, flatbuffers.UOffsetT(message), 0)
}
func WhisperEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
