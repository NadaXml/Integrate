namespace extension {
    public abstract class TemplateDefine {
        
        /// <summary>
        /// C#代码会把嵌套对象转成List<KeyValuePair>形式，然后通过模板代码导出
        /// 目前尚不支持对象中代列表
        /// </summary>
        public static readonly string ObjectKeyValueTemplate = @"
{{- func indentation -}}
    {{- for n in (0..$0) -}}
        {{- ""    "" -}}
    {{- end -}}
{{- end -}}

{{- func block_start -}}
    {{- ""{\n"" -}}
{{- end -}}

{{- func block_end -}}
    {{- ""}"" -}}
{{- end -}}

{{- func expandList -}}
    {{- if $0 | object.typeof == ""array"" -}}
        {{- for kv in $0 -}}
            {{- if kv.vv | object.typeof == ""array"" -}}
                {{- indentation $1 -}} {{ kv.kk }} = {{- block_start -}} {{ expandList kv.vv $1+1 }}
                {{- indentation $1 -}} {{- block_end -}} {{ ""\n"" }}
            {{- else -}}
                {{- indentation $1 -}} {{ kv.kk }} = {{ expandList kv.vv $1+1 }} {{ ""\n"" }}
            {{- end -}}
        {{- end -}}
    {{- else -}}
        {{- $0 -}}
    {{- end -}}
{{- end -}}

{{- expandList model 0 -}}
";
    }
}
