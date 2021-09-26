function countChar(val, maxLen) {
    let len = val.value.length;
    $(val).parent().find("#charNum").text(`${len}/${maxLen}`);
}