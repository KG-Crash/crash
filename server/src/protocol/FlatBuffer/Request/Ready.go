// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Request

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

func ReadyStart(builder *flatbuffers.Builder) {
	builder.StartObject(0)
}
func ReadyEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
