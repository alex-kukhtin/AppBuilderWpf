define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const template = {
        options: {
            noDirty: true
        },
        events: {
            'doc.saved': docSaved
        }
    };
    exports.default = template;

    function docSaved(doc) {
        let f = this.Documents.find(d => d.Id === doc.Id);
        if (f)
            f.$merge(doc);
    }
});
