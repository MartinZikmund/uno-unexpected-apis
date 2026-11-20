// Adapted from the Silk.NET OpenGL tutorials (MIT License):
// https://github.com/dotnet/Silk.NET/tree/c27224cce6b8136224c01d40de2d608879d709b5/examples/CSharp/OpenGL%20Tutorials
// Copyright (c) 2019-2020 Ultz Limited, (c) 2021- .NET Foundation and Contributors

#if DESKTOP || WINDOWS || __WASM__
using System.Diagnostics;
using System.Numerics;
using Silk.NET.OpenGL;
using Uno.WinUI.Graphics3DGL;

namespace UnexpectedApis.Apis;

public sealed class RotatingCubeGlCanvasElement() : GLCanvasElement(() => App.MainWindow!)
{
    private const string VertexShaderSource =
        """
        precision highp float; // for OpenGL ES compatibility

        layout(location = 0) in vec3 pos;
        layout(location = 1) in vec3 vertex_color;

        uniform mat4 transform;

        out vec3 color;

        void main() {
          gl_Position = transform * vec4(pos, 1.0);
          color = vertex_color;
        }
        """;

    private const string FragmentShaderSource =
        """
        precision highp float; // for OpenGL ES compatibility

        in vec3 color;

        out vec4 frag_color;

        void main() {
          frag_color = vec4(color, 1.0);
        }
        """;

    private static readonly float[] _vertices =
    [
        // Front face     // colors
        0.5f, 0.5f, 0.5f, 1.0f, 0.4f, 0.6f,
        -0.5f, 0.5f, 0.5f, 1.0f, 0.9f, 0.2f,
        -0.5f, -0.5f, 0.5f, 0.7f, 0.3f, 0.8f,
        0.5f, -0.5f, 0.5f, 0.5f, 0.3f, 1.0f,
        // Back face      // colors
        0.5f, 0.5f, -0.5f, 0.2f, 0.6f, 1.0f,
        -0.5f, 0.5f, -0.5f, 0.6f, 1.0f, 0.4f,
        -0.5f, -0.5f, -0.5f, 0.6f, 0.8f, 0.8f,
        0.5f, -0.5f, -0.5f, 0.4f, 0.8f, 0.8f,
    ];

    private static readonly uint[] _indices =
    [
        0, 1, 2, 2, 3, 0, // front
        0, 3, 7, 7, 4, 0, // right
        2, 6, 7, 7, 3, 2, // bottom
        1, 5, 6, 6, 2, 1, // left
        4, 7, 6, 6, 5, 4, // back
        5, 1, 0, 0, 4, 5, // top
    ];

    private uint _vao;
    private uint _vbo;
    private uint _ebo;
    private uint _program;
    private int _transformLocation = -1;
    private bool _ready;
    private bool _running = true;

    /// <summary>Stops the render loop; call it when the demo unloads.</summary>
    public void Stop() => _running = false;

    public void Start()
    {
        _running = true;
        Invalidate();
    }

    protected override unsafe void Init(GL gl)
    {
        _ready = false;
        try
        {
            _vao = gl.GenVertexArray();
            gl.BindVertexArray(_vao);

            _ebo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
            gl.BufferData(BufferTargetARB.ElementArrayBuffer, new ReadOnlySpan<uint>(_indices), BufferUsageARB.StaticDraw);

            _vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            gl.BufferData(BufferTargetARB.ArrayBuffer, new ReadOnlySpan<float>(_vertices), BufferUsageARB.StaticDraw);

            const uint stride = 6 * sizeof(float);
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, (void*)0);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, (void*)(3 * sizeof(float)));
            gl.EnableVertexAttribArray(1);

            var version = gl.GetStringS(StringName.ShadingLanguageVersion)
                .Contains("OpenGL ES", StringComparison.InvariantCultureIgnoreCase)
                ? "#version 300 es"
                : "#version 330";

            _program = BuildProgram(gl, version + Environment.NewLine + VertexShaderSource, version + Environment.NewLine + FragmentShaderSource);
            _transformLocation = gl.GetUniformLocation(_program, "transform");
            _ready = _program != 0 && _transformLocation >= 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Cube init failed: {ex.Message}");
        }
    }

    protected override void OnDestroy(GL gl)
    {
        _ready = false;
        try
        {
            gl.DeleteProgram(_program);
            gl.DeleteBuffer(_vbo);
            gl.DeleteBuffer(_ebo);
            gl.DeleteVertexArray(_vao);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Cube cleanup failed: {ex.Message}");
        }
    }

    protected override unsafe void RenderOverride(GL gl)
    {
        gl.Enable(EnableCap.DepthTest);
        gl.DepthMask(true);
        gl.ClearColor(0.1f, 0.12f, 0.2f, 1);
        gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        if (!_ready)
        {
            return;
        }

        gl.BindVertexArray(_vao);
        gl.UseProgram(_program);

        const double duration = 4;
        var transform =
            Matrix4x4.CreateRotationY((float)(Stopwatch.GetElapsedTime(0).TotalSeconds / duration * 2 * Math.PI)) *
            Matrix4x4.CreateRotationX((float)(0.15 * Math.PI)) *
            Matrix4x4.CreateTranslation(0, 0, -3) *
            Perspective();

        gl.UniformMatrix4(_transformLocation, 1, false, (float*)&transform);
        gl.DrawElements(PrimitiveType.Triangles, (uint)_indices.Length, DrawElementsType.UnsignedInt, null);

        if (_running)
        {
            Invalidate();
        }
    }

    // https://www.songho.ca/opengl/gl_projectionmatrix.html
    private static Matrix4x4 Perspective()
    {
        const float r = 0.5f, t = 0.5f, n = 1, f = 5;
        return new Matrix4x4(
            n / r, 0, 0, 0,
            0, n / t, 0, 0,
            0, 0, (-f - n) / (f - n), -1,
            0, 0, 2 * f * n / (n - f), 0);
    }

    private static uint BuildProgram(GL gl, string vertexSource, string fragmentSource)
    {
        var vertex = Compile(gl, ShaderType.VertexShader, vertexSource);
        var fragment = Compile(gl, ShaderType.FragmentShader, fragmentSource);
        var program = gl.CreateProgram();
        gl.AttachShader(program, vertex);
        gl.AttachShader(program, fragment);
        gl.LinkProgram(program);
        gl.GetProgram(program, GLEnum.LinkStatus, out var status);
        gl.DetachShader(program, vertex);
        gl.DetachShader(program, fragment);
        gl.DeleteShader(vertex);
        gl.DeleteShader(fragment);

        if (status == 0)
        {
            var log = gl.GetProgramInfoLog(program);
            gl.DeleteProgram(program);
            throw new InvalidOperationException($"Program failed to link: {log}");
        }

        return program;
    }

    private static uint Compile(GL gl, ShaderType type, string source)
    {
        var shader = gl.CreateShader(type);
        gl.ShaderSource(shader, source);
        gl.CompileShader(shader);
        var log = gl.GetShaderInfoLog(shader);
        if (!string.IsNullOrWhiteSpace(log))
        {
            gl.DeleteShader(shader);
            throw new InvalidOperationException($"{type} failed to compile: {log}");
        }

        return shader;
    }
}
#endif
