#!/usr/bin/env zx

function a()
{
    const oldText = 'inline constexpr char kMetadataParserVersion[] = "{LATEST_METADATA_PARSER_VERSION}";';
    const newText = 'inline constexpr char kMetadataParserVersion[] = "1.5.0";';

    fs.readdir(".", { recursive: true }, (err, files) => {
        files.filter(x => x.endsWith(".h.template")).forEach(async file => {
            console.log(file);

            const content = await fs.readFile(file, 'utf-8');
            const newContent = content.replace(new RegExp(oldText, "g"), newText);

            if (content !== newContent) {
                console.log("Patch " + file);
                await fs.writeFile(file, newContent);
            }
        });
    });
}

function b()
{
    const oldText = 'build:linux --define=xnn_enable_avx512amx=false\r\n\r\n';
    const newText = 'build:linux --define=xnn_enable_avx512amx=false\r\nbuild:linux --define=xnn_enable_avx512fp16=false\r\n\r\n';

    fs.readdir(".", { recursive: true }, (err, files) => {
        files.filter(x => x.endsWith(".bazelrc")).forEach(async file => {
            console.log(file);

            const content = await fs.readFile(file, 'utf-8');
            const newContent = content.replace(new RegExp(oldText, "g"), newText);

            if (content !== newContent) {
                console.log("Patch " + file);
                await fs.writeFile(file, newContent);
            }
        });
    });
}

a();
b();

console.log('Monkey patching is done');

