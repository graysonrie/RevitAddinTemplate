"use client";
import { invoke } from "./interop";

export default function Home() {
  const buttonClick = async () => {
    const result = await invoke<string>("ButtonClick", { Args: "Hello" });
    console.log(result);
  };

  return (
    <div className="flex flex-col items-center justify-center h-screen">
      <h1 className="text-4xl font-bold">Hello World</h1>
      <button
        className="bg-blue-500 text-white p-2 rounded-md"
        onClick={buttonClick}
      >
        Click me
      </button>
    </div>
  );
}
