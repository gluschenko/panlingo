#!/usr/bin/env zx

async function findAndPatch(endsWith, oldText, newText) {
    const files = fs.readdirSync(".", { recursive: true });

    for (const file of files.filter(x => x.endsWith(endsWith))) {
        console.log("Found: " + file);

        const content = await fs.readFile(file, 'utf-8');
        const newContent = content.replaceAll(oldText, newText);

        if (content !== newContent) {
            await fs.writeFile(file, newContent);
            console.log("Patched: " + file + "\nContent:\n" + newContent);
        }
        else {
            console.log("Nothing to patch: " + file);
        }
    }
}

function a()
{
    const oldText = '#include "cld_3/protos/';
    const newText = '// #include "cld_3/protos/';

    findAndPatch(".h", oldText, newText);
    findAndPatch(".cc", oldText, newText);
}

function b()
{
    const oldText = '#include <vector>\n';
    const newText = '#include <vector>\n#include "fake_protobuf.h"\n';

    findAndPatch("base.h", oldText, newText);
}

console.log('[Monkey patching is started]');

await a();
await b();

console.log('[Monkey patching is done]');

