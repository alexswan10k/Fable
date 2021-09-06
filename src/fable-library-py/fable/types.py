from __future__ import annotations

from abc import abstractstaticmethod
from typing import Any, Generic, Iterable, List, Tuple, TypeVar, Union as Union_, Callable, Optional, cast
from .util import equals

from .util import IComparable

T = TypeVar("T")


class FSharpRef(Generic[T]):
    def __init__(self, contentsOrGetter, setter=None) -> None:

        contents = contentsOrGetter

        def set_contents(value):
            nonlocal contents
            contents = value

        if callable(setter):
            self.getter = contentsOrGetter
            self.setter = setter
        else:
            self.getter = lambda: contents
            self.setter = set_contents

    @property
    def contents(self) -> T:
        return self.getter()

    @contents.setter
    def contents(self, v) -> None:
        self.setter(v)


class Union(IComparable):
    def __init__(self):
        self.tag: int
        self.fields: Tuple[int, ...] = ()

    @abstractstaticmethod
    def cases() -> List[str]:
        ...

    @property
    def name(self) -> str:
        return self.cases()[self.tag]

    def to_JSON(self) -> str:
        raise NotImplementedError
        # return str([self.name] + self.fields) if len(self.fields) else self.name

    def __str__(self) -> str:
        if not len(self.fields):
            return self.name

        fields = ""
        with_parens = True
        if len(self.fields) == 1:
            field = str(self.fields[0])
            with_parens = field.find(" ") >= 0
            fields = field
        else:
            fields = ", ".join(map(str, self.fields))

        return self.name + (" (" if with_parens else " ") + fields + (")" if with_parens else "")

    def __hash__(self) -> int:
        hashes = map(hash, self.fields)
        return hash([hash(self.tag), *hashes])

    def __eq__(self, other: Any) -> bool:
        if self is other:
            return True
        if not isinstance(other, Union):
            return False

        if self.tag == other.tag:
            return self.fields == other.fields

        return False

    def __lt__(self, other: Any) -> bool:
        if self.tag == other.tag:
            return self.fields < other.fields

        return self.tag < other.tag


def recordEquals(self, other):
    if self is other:
        return True

    for name in self.__dict__.keys():
        if not equals(self.__dict__[name], other.__dict__[name]):
            return False

    return True


def recordCompareTo(self, other):
    if self is other:
        return 0

    else:
        for name in self.__dict__.keys():
            if self.__dict__[name] < other.__dict__.get(name):
                return -1
            elif self.__dict__[name] > other.__dict__.get(name):
                return 1

        return 0


def recordToString(self):
    return "{ " + "\n  ".join(map(lambda kv: kv[0] + " = " + str(kv[1]), self.__dict__.items())) + " }"


def recordGetHashCode(self):
    return hash(*self.values())


class Record(IComparable):
    def toJSON(self) -> str:
        return record_to_JSON(self)

    def __str__(self) -> str:
        return recordToString(self)

    def GetHashCode(self) -> int:
        return recordGetHashCode(self)

    def Equals(self, other: Record) -> bool:
        return recordEquals(self, other)

    def CompareTo(self, other: Record) -> int:
        return recordCompareTo(self, other)

    def __lt__(self, other: Any) -> bool:
        return True if self.CompareTo(other) == -1 else False

    def __eq__(self, other: Any) -> bool:
        return self.Equals(other)

    def __hash__(self) -> int:
        return recordGetHashCode(self)


class Attribute:
    pass


def seq_to_string(self):
    str = "["

    for count, x in enumerate(self):
        if count == 0:
            str += to_string(x)

        elif count == 100:
            str += "; ..."
            break

        else:
            str += "; " + to_string(x)

    return str + "]"


def to_string(x, callStack=0):
    if x is not None:
        # if (typeof x.toString === "function") {
        #    return x.toString();

        if isinstance(x, str):
            return str(x)

        if isinstance(x, Iterable):
            return seq_to_string(x)

        # else: // TODO: Date?
        #     const cons = Object.getPrototypeOf(x).constructor;
        #     return cons === Object && callStack < 10
        #         // Same format as recordToString
        #         ? "{ " + Object.entries(x).map(([k, v]) => k + " = " + toString(v, callStack + 1)).join("\n  ") + " }"
        #         : cons.name;

    return str(x)


class Exception(Exception):
    def __init__(self, msg=None):
        self.msg = msg

    def __eq__(self, other):
        if self is other:
            return True

        if other is None:
            return False

        return self.msg == other.msg


class FSharpException(Exception, IComparable):
    def __init__(self):
        self.Data0: Any = None

    def toJSON(self):
        return record_to_JSON(self)

    def __str__(self):
        return recordToString(self)

    def __eq__(self, other):
        if self is other:
            return True

        if other is None:
            return False

        return self.Data0 == other.Data0

    def __lt__(self, other: Any) -> bool:
        if not isinstance(other, FSharpException):
            return False

        if self.Data0:
            if other.Data0:
                return self.Data0 < other.Data0
            else:
                return False

        elif not self.Data0:
            if other.Data0:
                return False
            else:
                return True

        return super().__lt__(other)

    def __hash__(self) -> int:
        return hash(self.Data0)

    def GetHashCode(self):
        recordGetHashCode(self)

    def Equals(self, other: FSharpException):
        return recordEquals(self, other)

    def CompareTo(self, other: FSharpException):
        return recordCompareTo(self, other)


__all__ = ["Attribute", "Exception", "FSharpRef", "to_string", "Union"]
