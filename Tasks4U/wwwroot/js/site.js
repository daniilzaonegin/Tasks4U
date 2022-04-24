function countChar(val, maxLen) {
    const len = val.value.length;
    $(val).parent().find("#charNum").text(`${len}/${maxLen}`);
}