var HtmlWebpackPlugin = require("html-webpack-plugin");
const path = require("path");

module.exports = {
    mode: "development",
    module: {
        rules: [
            { test: /\.jsx?$/, loader: "babel-loader" },
            { test: /\.(png|jpe?g|gif|webp)$/, use: [{ loader: "file-loader" }] },
            { test: /\.less$/, use: ["style-loader", "css-loader", "less-loader"] },
        ],
    },
    resolve: {
        mainFiles: ["index", "Index"],
        extensions: [".js", ".jsx", ".json"], // fix: include dot
        alias: {
            config: path.resolve(__dirname, "src/config"),
            "@": path.resolve(__dirname, "src"),
        },
    },
    plugins: [new HtmlWebpackPlugin({ template: "./src/index.html" })],
    devServer: {
        historyApiFallback: true,

        // IMPORTANT if backend is a different host/port in dev:
        proxy: {
            "/api": {
                target: "http://localhost:5000",
                changeOrigin: true,
                secure: false,
            },
        },
    },
};
