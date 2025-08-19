import { redirect } from "next/navigation";
import { Button } from "@/components/ui/button";
import {
    Table,
    TableBody,
    TableCaption,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
import Link from "next/link";
import { Badge } from "@/components/ui/badge";
import { cookies } from "next/headers";

function TH({ children }) {
    return (
        <TableHead className={"text-center"}>
            {children}
        </TableHead>
    )
}

function Cell({ children }) {
    return (
        <TableCell className={"text-center"}>
            {children}
        </TableCell>
    )
}

function InvoiceButton({ children }) {
    return (
        <Button className={"bg-identity-dillute hover:bg-identity"}>
            {children}
        </Button>
    )
}

export default async function Invoice(props) {
    const cookieStore = cookies();
    const session = cookieStore.get("session")?.value;

    const res = await fetch("http://localhost:3000/api/invoices", {
        headers: {
            Cookie: `session=${session}`,
        },
        cache: "no-store",
    });

    let invoices;

    if (!res.ok) {
        if (res.status === 401) {
            // Clear session cookie by calling logout API
            // redirect("/api/auth/logout");
        }

        // throw new Error('Failed to load invoices');
    } else {
        invoices = await res.json();
    }

    invoices = invoices || [];

    return (
        <main className="flex flex-col h-min-full w-full p-4 lg:p-0 ">
            <div className="container mx-auto my-12">
                <h1 className="text-5xl font-bold mb-8">Invoices</h1>
                <p>Generate invoices with just a click of a button</p>
            </div>

            <div className="container mx-auto">
                <Link className="bg-identity object flex m-4 p-2 text-background rounded-sm w-fit" href={"invoices/new"}>Add New Invoice</Link>
            </div>
            {

                invoices.length > 0 ?
                    (<>
                        <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                            <TableHeader >
                                <TableRow className={"bg-accent rounded-xl"}>
                                    <TH>Customers</TH>
                                    <TH>Status</TH>
                                    <TH>Order Date

                                    </TH>
                                    <TH>Due Date</TH>
                                    <TH>Generate Invoice</TH>
                                    <TH>Send an email</TH>
                                    <TH>Receipt</TH>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {
                                    invoices.map((invoice, index) => (
                                        <TableRow key={index} >
                                            <Cell>
                                                {invoice.UserId}
                                            </Cell>
                                            <Cell>
                                                {invoice.Status}
                                            </Cell>
                                            <Cell>
                                                {invoice.OrderDate}
                                            </Cell>
                                            <Cell>
                                                {invoice.DueDate}
                                            </Cell>
                                            <Cell>
                                                <InvoiceButton>
                                                    Invoice
                                                </InvoiceButton>
                                            </Cell>
                                            <Cell>
                                                <InvoiceButton>send email</InvoiceButton>
                                            </Cell>
                                            <Cell>
                                                <Link
                                                    href={"/invoices"}>
                                                    {""}
                                                    {/* Later to be changed */}
                                                </Link>
                                            </Cell>
                                        </TableRow>
                                    ))
                                }


                            </TableBody>

                        </Table>
                    </>)
                    :
                    (<>
                        <Badge variant={"destructive"} className={"mx-auto"}>Failed to load invoices</Badge>
                        <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                            <TableHeader >
                                <TableRow className={"bg-accent rounded-xl"}>
                                    <TH>Customers</TH>
                                    <TH>Status</TH>
                                    <TH>Order Date

                                    </TH>
                                    <TH>Due Date</TH>
                                    <TH>Generate Invoice</TH>
                                    <TH>Send an email</TH>
                                    <TH>Receipt</TH>
                                </TableRow>
                            </TableHeader>
                            <TableBody></TableBody>
                        </Table>
                    </>)
            }

        </main>
    )
}

const dummyCustomers = [
    {
        InvoiceId: "INV0001",
        ClientId: "CS0001",
        UserId: "US0001",
        Status: "Cancelled",
        OrderDate: "DD/MM/YYYY",
        DueDate: "DD/MM/YYYY",
        TotalAmount: 1000,
        CreatedAt: "DD/MM/YYYY HH:MM:SS",

        InvoicePdfFileName: "",
        EmailMessageId: "",
        EmaiThreadId: "",
        InvoiceEmailSentAt: "",

        Client: {},
        User: {},
        Items: {},
        Receipts: {}
    },
];