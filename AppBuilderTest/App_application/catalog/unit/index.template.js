define(["require", "exports"], function (require, exports) {
    "use strict";

    Object.defineProperty(exports, "__esModule", { value: true });
    const template = {
        commands: {
            testToast,
            showInline
        }
    };
    exports.default = template;

    async function testToast() {
        this.$ctrl.$toast('I am the toast');

        await this.$ctrl.$alert('alert here');
        alert(await this.$ctrl.$confirm('Are you sure?'));
    }

    function showInline() {
        this.$ctrl.$inlineOpen('test');
    }

});

