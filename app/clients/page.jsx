import { Toaster } from "@/components/ui/sonner"
import { AppSidebar } from "@/components/AppSidebar";
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
import { getBackEndURL } from "@/lib/utils";

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

function ClientButton({ children }) {
    return (
        <Button className={"bg-identity-dillute hover:bg-identity"}>
            {children}
        </Button>
    )
}

export default async function Clients(props) {

    let Clients = [];
    try {
        const res = await fetch(`${getBackEndURL()}/api/Clients`, {
            method: "GET",
            cache: 'no-store'
        });
        // if (!res.ok) throw new Error('Failed to load Clients');
        if (res.ok)
            Clients = await res.json();
    } catch (e) {
        console.error("Failed to load Clients,", e);
        Clients = [];
    }
    return (
        <main className="flex flex-col h-min-full w-full p-4 lg:p-0 ">
            <div className="container mx-auto my-12">
                <h1 className="text-5xl font-bold mb-8">Clients</h1>
                <p>Generate Clients with just a click of a button</p>
            </div>
            <div className="container mx-auto">
                <Link className="bg-identity object m-4 p-2 text-background rounded-sm w-fit" href={"clients/new"}>Add New Client</Link>
            </div>


            {

                Clients.length > 0 ?
                    (<>
                        <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                            <TableHeader >
                                <TableRow className={"bg-accent rounded-xl"}>
                                    <TH>Customers</TH>
                                    <TH>Status</TH>
                                    <TH>Order Date

                                    </TH>
                                    <TH>Due Date</TH>
                                    <TH>Generate Client</TH>
                                    <TH>Send an email</TH>
                                    <TH>Receipt</TH>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {
                                    Clients.map((Client, index) => (
                                        <TableRow key={index} >
                                            <Cell>
                                                {Client.UserId}
                                            </Cell>
                                            <Cell>
                                                {Client.Status}
                                            </Cell>
                                            <Cell>
                                                {Client.OrderDate}
                                            </Cell>
                                            <Cell>
                                                {Client.DueDate}
                                            </Cell>
                                            <Cell>
                                                <ClientButton>
                                                    Client
                                                </ClientButton>
                                            </Cell>
                                            <Cell>
                                                <ClientButton>send email</ClientButton>
                                            </Cell>
                                            <Cell>
                                                <Link
                                                    href={"/clients"}>
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
                        <Badge variant={"destructive"} className={"mx-auto"}>Failed to load Clients</Badge>
                        <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                            <TableHeader >
                                <TableRow className={"bg-accent rounded-xl"}>
                                    <TH>Customers</TH>
                                    <TH>Status</TH>
                                    <TH>Order Date

                                    </TH>
                                    <TH>Due Date</TH>
                                    <TH>Generate Client</TH>
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
        ClientId: "INV0001",
        ClientId: "CS0001",
        UserId: "US0001",
        Status: "Cancelled",
        OrderDate: "DD/MM/YYYY",
        DueDate: "DD/MM/YYYY",
        TotalAmount: 1000,
        CreatedAt: "DD/MM/YYYY HH:MM:SS",

        ClientPdfFileName: "",
        EmailMessageId: "",
        EmaiThreadId: "",
        ClientEmailSentAt: "",

        Client: {},
        User: {},
        Items: {},
        Receipts: {}
    },
];