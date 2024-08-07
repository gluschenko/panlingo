#!/usr/bin/env zx

import { find } from 'zx'

{
    const oldText = 'inline constexpr char kMetadataParserVersion[] = "{LATEST_METADATA_PARSER_VERSION}";';
    const newText = 'inline constexpr char kMetadataParserVersion[] = "1.5.0";';

    const files = await find('**/*.cc');

    for (const file of files) {
        const content = await fs.readFile(file, 'utf-8');
        const newContent = content.replace(new RegExp(oldText, 'g'), newText);

        if (content !== newContent) {
            await fs.writeFile(file, content);
        }
    }
}

{
    const oldText = 'build:linux --define=xnn_enable_avx512amx=false\n';
    const newText = 'build:linux --define=xnn_enable_avx512amx=false\nbuild:linux --define=xnn_enable_avx512fp16=false\n';

    const files = await find('**/.bazelrc');

    for (const file of files) {
        const content = await fs.readFile(file, 'utf-8');
        const newContent = content.replace(new RegExp(oldText, 'g'), newText);

        if (content !== newContent) {
            await fs.writeFile(file, content);
        }
    }
}

console.log('Monkey patching is done');

