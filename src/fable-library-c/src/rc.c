#include <stdlib.h>

struct Rc {
    void *data;
    //todo function pointer to data.Dispose()
    //int (*dispose) ();
    int size;
    int *count;
};

struct Rc Rc_New(int size, void *data) {
    struct Rc rc;
    rc.count = malloc(sizeof(int));
    *rc.count = 1;
    rc.data = malloc(size);
    rc.size = size;
    memcpy(rc.data, data, size);
    // rc.Data =
    return rc;
};

struct Rc Rc_Clone(struct Rc value) {
    //struct Rc* rc = (struct Rc*) value;
    *value.count = *value.count + 1;
    struct Rc next;
    next.count = value.count;
    next.data = value.data;
    next.size = value.size;
    return next;
}

int Rc_Dispose(struct Rc value) {
    // struct Rc* rc = (struct Rc*) value;
    *value.count = *value.count - 1;
    if(*value.count == 0){
        free(value.data);
        free(value.count);
    }
    return *value.count;
}

// how to use

// This is a .NET reference type
struct __Example_Use_Rc_Struct {
    int X;
};

int __example_use_rc() {
    //Create a new instance
    struct __Example_Use_Rc_Struct test;
    test.X = 1;
    struct Rc rc = Rc_New(sizeof(test), &test);

    //Leave context, ownership
    struct Rc rc2 = Rc_Clone(rc);

    //dereference
    int outVal = ((struct __Example_Use_Rc_Struct *)(rc2.data))->X;
    // (Test*)

    //Go out of scope, clean up
    Rc_Dispose(rc);
    Rc_Dispose(rc2);

    return 1;
}

struct __Example_Use_Rc_Struct_Complex {
    struct Rc a; //__Example_Use_Rc_Struct
};