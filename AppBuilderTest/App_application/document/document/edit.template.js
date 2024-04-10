define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const template = {
        events: {
            'Model.saved': modelSaved
        }
    };
    exports.default = template;

    function modelSaved() {
        this.$ctrl.$emitParentTab('doc.saved', this.Document);
    }
});
