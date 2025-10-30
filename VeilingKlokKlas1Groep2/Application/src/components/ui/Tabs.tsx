export const Tabs = ({ children }: any) => <div>{children}</div>;
export const TabsList = ({ children }: any) => (
  <div className="flex gap-2">{children}</div>
);
export const TabsTrigger = ({ children, ...props }: any) => (
  <button className="px-3 py-1 border rounded" {...props}>
    {children}
  </button>
);
export const TabsContent = ({ children }: any) => <div>{children}</div>;
