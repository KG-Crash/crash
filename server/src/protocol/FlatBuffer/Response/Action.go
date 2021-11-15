// Code generated by the FlatBuffers compiler. DO NOT EDIT.

package Response

import (
	flatbuffers "github.com/google/flatbuffers/go"
)

type Action struct {
	_tab flatbuffers.Table
}

func GetRootAsAction(buf []byte, offset flatbuffers.UOffsetT) *Action {
	n := flatbuffers.GetUOffsetT(buf[offset:])
	x := &Action{}
	x.Init(buf, n+offset)
	return x
}

func (rcv *Action) Init(buf []byte, i flatbuffers.UOffsetT) {
	rcv._tab.Bytes = buf
	rcv._tab.Pos = i
}

func (rcv *Action) Table() flatbuffers.Table {
	return rcv._tab
}

func (rcv *Action) Frame() int32 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(4))
	if o != 0 {
		return rcv._tab.GetInt32(o + rcv._tab.Pos)
	}
	return 0
}

func (rcv *Action) MutateFrame(n int32) bool {
	return rcv._tab.MutateInt32Slot(4, n)
}

func (rcv *Action) Id() int32 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(6))
	if o != 0 {
		return rcv._tab.GetInt32(o + rcv._tab.Pos)
	}
	return 0
}

func (rcv *Action) MutateId(n int32) bool {
	return rcv._tab.MutateInt32Slot(6, n)
}

func (rcv *Action) PositionX() int32 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(8))
	if o != 0 {
		return rcv._tab.GetInt32(o + rcv._tab.Pos)
	}
	return 0
}

func (rcv *Action) MutatePositionX(n int32) bool {
	return rcv._tab.MutateInt32Slot(8, n)
}

func (rcv *Action) PositionY() int32 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(10))
	if o != 0 {
		return rcv._tab.GetInt32(o + rcv._tab.Pos)
	}
	return 0
}

func (rcv *Action) MutatePositionY(n int32) bool {
	return rcv._tab.MutateInt32Slot(10, n)
}

func ActionStart(builder *flatbuffers.Builder) {
	builder.StartObject(4)
}
func ActionAddFrame(builder *flatbuffers.Builder, frame int32) {
	builder.PrependInt32Slot(0, frame, 0)
}
func ActionAddId(builder *flatbuffers.Builder, id int32) {
	builder.PrependInt32Slot(1, id, 0)
}
func ActionAddPositionX(builder *flatbuffers.Builder, positionX int32) {
	builder.PrependInt32Slot(2, positionX, 0)
}
func ActionAddPositionY(builder *flatbuffers.Builder, positionY int32) {
	builder.PrependInt32Slot(3, positionY, 0)
}
func ActionEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
