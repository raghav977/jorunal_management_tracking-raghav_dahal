// Quill.js Blazor Interop
// Provides JavaScript functions for Blazor to interact with Quill rich text editor

window.QuillInterop = {
    quill: null,

    // Initialize Quill editor on the specified element
    init: function (elementId, initialContent) {
        const container = document.getElementById(elementId);
        if (!container) {
            console.error('Quill container not found:', elementId);
            return;
        }

        // Create Quill instance with Snow theme
        this.quill = new Quill('#' + elementId, {
            theme: 'snow',
            placeholder: 'Write your thoughts...',
            modules: {
                toolbar: [
                    [{ 'header': [1, 2, 3, false] }],
                    ['bold', 'italic', 'underline', 'strike'],
                    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                    ['link'],
                    ['clean']
                ],
                keyboard: {
                    bindings: {
                        // Default keyboard bindings are already included
                        // Cmd/Ctrl+B for bold, Cmd/Ctrl+I for italic, etc.
                    }
                }
            }
        });

        // Set initial content if provided
        if (initialContent) {
            this.quill.root.innerHTML = initialContent;
        }
    },

    // Get HTML content from the editor
    getContent: function () {
        if (!this.quill) {
            return '';
        }
        const content = this.quill.root.innerHTML;
        // Return empty string if only contains empty paragraph
        if (content === '<p><br></p>') {
            return '';
        }
        return content;
    },

    // Set HTML content in the editor
    setContent: function (html) {
        if (this.quill) {
            this.quill.root.innerHTML = html || '';
        }
    },

    // Get plain text content (for word count)
    getText: function () {
        if (!this.quill) {
            return '';
        }
        return this.quill.getText().trim();
    },

    // Focus the editor
    focus: function () {
        if (this.quill) {
            this.quill.focus();
        }
    },

    // Destroy the Quill instance (cleanup)
    destroy: function () {
        this.quill = null;
    }
};
